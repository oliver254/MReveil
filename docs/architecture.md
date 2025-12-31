# Architecture de MReveil

## 🏗️ Vue d'ensemble de l'architecture

MReveil suit une architecture en couches basée sur le pattern MVVM (Model-View-ViewModel) avec une injection de dépendances.

```mermaid
graph TB
    subgraph "Couche Présentation"
        V[Views XAML]
        C[Controls personnalisés]
    end
    
    subgraph "Couche ViewModel"
        VM[ViewModels]
        M[Messaging]
    end
    
    subgraph "Couche Service"
        TS[ThemeService]
        TM[TimerManager]
        SS[SettingsService]
        DS[DatabaseService]
        StS[StatisticsService]
        PS[PomodoroSessionService]
    end
    
    subgraph "Couche Modèle"
        MD[Models de données]
        ST[States]
    end
    
    subgraph "Couche Données"
        DB[(SQLite)]
        PREFS[Preferences]
    end
    
    V --> VM
    C --> VM
    VM --> M
    VM --> TS
    VM --> TM
    VM --> SS
    VM --> DS
    VM --> StS
    VM --> PS
    TS --> PREFS
    SS --> PREFS
    DS --> DB
    StS --> DS
    PS --> DS
    TM --> ST
    VM --> MD
```

## 📦 Composants principaux

### 1. Couche Présentation (Views)

#### Pages principales
- **MainPage** - Page d'accueil avec le timer Pomodoro
- **JournalPage** - Page de journal quotidien
- **StatisticsPage** - Page de statistiques et graphiques
- **SettingsPage** - Page de configuration

#### Contrôles personnalisés
- **CircularClock** - Horloge circulaire animée avec affichage visuel du temps

### 2. Couche ViewModel

```mermaid
classDiagram
    class ObservableObject {
        <<abstract>>
        +PropertyChanged
        +OnPropertyChanged()
    }
    
    class MainViewModel {
        -TimerManager _timerManager
        -PomodoroSessionService _pomodoroSessionService
        -SettingsViewModel _settings
        +IState State
        +bool Alarm
        +SetDuration(ActivityType)
        +Pause()
        +Stop()
    }
    
    class JournalViewModel {
        -DatabaseService _databaseService
        +JournalEntry CurrentEntry
        +ObservableCollection~JournalEntry~ JournalEntries
        +DateTime SelectedDate
        +SaveJournalEntry()
        +LoadJournalEntry()
    }
    
    class StatisticsViewModel {
        -StatisticsService _statisticsService
        +int TodayPomodoros
        +int TodayFocusMinutes
        +int CurrentStreak
        +LoadTodayStats()
    }
    
    class SettingsViewModel {
        -SettingsService _settingsService
        -ThemeService _themeService
        +int SprintDuration
        +int ShortBreakDuration
        +int LongBreakDuration
        +bool LoopPlayback
        +string Mode
    }
    
    ObservableObject <|-- MainViewModel
    ObservableObject <|-- JournalViewModel
    ObservableObject <|-- StatisticsViewModel
    ObservableObject <|-- SettingsViewModel
```

### 3. Couche Service

```mermaid
classDiagram
    class TimerManager {
        -IState _state
        +Play(TimeSpan)
        +Pause()
        +Stop()
        +State IState
    }
    
    class DatabaseService {
        -SQLiteAsyncConnection _connection
        -SemaphoreSlim _initSemaphore
        -bool _initialized
        +InitializeAsync()
        +SaveSessionAsync(PomodoroSession)
        +GetSessionsByDateAsync(DateTime)
        +SaveJournalEntryAsync(JournalEntry)
        +GetJournalEntryByDateAsync(DateTime)
    }
    
    class PomodoroSessionService {
        -DatabaseService _databaseService
        -PomodoroSession _currentSession
        +StartSession(ActivityType, int)
        +CompleteSessionAsync(int)
        +GetCurrentSession()
    }
    
    class StatisticsService {
        -DatabaseService _databaseService
        +GetTodayStatisticsAsync()
        +GetMonthlyStatisticsAsync(int, int)
        +GetCurrentStreakAsync()
    }
    
    class SettingsService {
        +int SprintDuration
        +int ShortBreakDuration
        +int LongBreakDuration
        +bool LoopPlayback
        +string Mode
    }
    
    class ThemeService {
        +Initialize()
        +SetTheme(AppTheme)
        +ToggleTheme()
    }
    
    PomodoroSessionService --> DatabaseService
    StatisticsService --> DatabaseService
```

### 4. Couche Modèle

```mermaid
classDiagram
    class IState {
        <<interface>>
        +TimeSpan Time
        +Refresh()
    }
    
    class ClockState {
        +TimeSpan Time
        +Refresh()
    }
    
    class CountdownState {
        -DateTime End
        -bool Alarm
        +TimeSpan Time
        +Refresh()
    }
    
    class PomodoroSession {
        +int Id
        +DateTime Date
        +ActivityType Type
        +int PlannedDuration
        +int ActualDuration
        +bool IsCompleted
        +string Notes
        +DateTime CreatedAt
        +DateTime? CompletedAt
    }
    
    class JournalEntry {
        +int Id
        +DateTime Date
        +string Title
        +string Content
        +int PomodorosCompleted
        +int TotalFocusMinutes
        +string Mood
        +DateTime CreatedAt
        +DateTime? UpdatedAt
    }
    
    class ActivityType {
        <<enumeration>>
        Pomodoro
        ShortBreak
        LongBreak
    }
    
    IState <|.. ClockState
    IState <|.. CountdownState
    PomodoroSession --> ActivityType
```

## 🔄 Flux de données

### Démarrage d'une session Pomodoro

```mermaid
sequenceDiagram
    participant U as User
    participant V as MainPage
    participant VM as MainViewModel
    participant PS as PomodoroSessionService
    participant TM as TimerManager
    participant DB as DatabaseService
    
    U->>V: Clique sur "🍅 Pomodoro"
    V->>VM: SetDurationCommand(Pomodoro)
    VM->>PS: StartSession(Pomodoro, 25)
    PS->>DB: SaveSessionAsync(session)
    DB-->>PS: Session enregistrée
    VM->>TM: Play(TimeSpan.FromMinutes(25))
    TM-->>VM: State = CountdownState
    VM-->>V: Mise à jour de la vue
    V-->>U: Timer démarre
```

### Cycle de rafraîchissement du timer

```mermaid
sequenceDiagram
    participant CC as CircularClock
    participant T as Timer (IDispatcherTimer)
    participant S as IState
    participant D as CircularDrawable
    
    loop Toutes les 100ms
        T->>CC: OnTimerTick()
        CC->>S: Refresh()
        S-->>CC: Time mis à jour
        CC->>CC: UpdateDisplay(time)
        CC->>D: Minute = time.Minutes
        CC->>D: Second = time.Seconds
        CC->>D: Invalidate()
        D-->>CC: Canvas redessiné
    end
    
    alt Time <= 0
        CC->>CC: OnAlarm()
        CC->>VM: Event Alarm
        VM->>M: Send AlarmMessage
    end
```

### Enregistrement d'une entrée de journal

```mermaid
sequenceDiagram
    participant U as User
    participant V as JournalPage
    participant VM as JournalViewModel
    participant DB as DatabaseService
    participant SQLite as SQLite DB
    
    U->>V: Saisit du contenu
    U->>V: Clique "Save"
    V->>VM: SaveJournalEntryCommand
    VM->>VM: Prépare JournalEntry
    VM->>DB: SaveJournalEntryAsync(entry)
    DB->>DB: EnsureInitializedAsync()
    DB->>SQLite: InsertAsync(entry)
    SQLite-->>DB: Id de l'entrée
    DB-->>VM: Succès
    VM-->>V: Notification de succès
    V-->>U: "Enregistré ✓"
```

## 🎨 Pattern de communication

### Messaging avec WeakReferenceMessenger

```mermaid
graph LR
    subgraph "Émetteurs"
        CS[CountdownState]
        VM[MainViewModel]
    end
    
    subgraph "Messager"
        WRM[WeakReferenceMessenger]
    end
    
    subgraph "Récepteurs"
        MP[MainPage]
        CC[CircularClock]
    end
    
    CS -->|AlarmMessage| WRM
    VM -->|ResetAlarmMessage| WRM
    VM -->|StateUpdateMessage| WRM
    WRM -->|Subscribe| MP
    WRM -->|Subscribe| CC
```

## 🗄️ Gestion de la base de données

### Initialisation lazy et thread-safe

```mermaid
stateDiagram-v2
    [*] --> NonInitialisé
    NonInitialisé --> Initialisation: Première requête
    Initialisation --> Attente: Autre thread
    Attente --> Initialisé: Libération du sémaphore
    Initialisation --> Initialisé: Création tables
    Initialisé --> Initialisé: Requêtes
    
    note right of Initialisation
        SemaphoreSlim protège
        contre les initialisations
        concurrentes
    end note
```

## 🎯 Principes d'architecture

### 1. Séparation des responsabilités
- **Views** : Uniquement l'interface utilisateur
- **ViewModels** : Logique de présentation et état
- **Services** : Logique métier
- **Models** : Données et règles métier

### 2. Injection de dépendances
- Configuration dans `MauiProgram.cs`
- Toutes les dépendances enregistrées comme singletons
- Résolution automatique par le conteneur DI

### 3. Communication découplée
- Messages faibles pour éviter les fuites mémoire
- Événements pour les contrôles
- PropertyChanged pour le binding MVVM

### 4. Gestion asynchrone
- Toutes les opérations de base de données async
- Initialisation lazy pour éviter les blocages
- Dispose pattern pour la libération des ressources

## 🔒 Gestion de la concurrence

- **SemaphoreSlim** pour la synchronisation de l'initialisation DB
- **Task.Run** pour les opérations longues
- **ConfigureAwait(false)** pour éviter les deadlocks (si nécessaire)

## 📊 Performances

### Optimisations
- Timer à 100ms (au lieu de temps réel)
- Invalidation du canvas uniquement si changement
- Initialisation lazy de la base de données
- Requêtes SQLite indexées sur les dates

### Mémoire
- WeakReferenceMessenger pour éviter les fuites
- Dispose pattern sur les contrôles
- Désinscription des événements dans OnUnloaded
