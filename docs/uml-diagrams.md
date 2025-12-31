# Diagrammes UML - MReveil

Ce document contient tous les diagrammes UML 2 du projet MReveil, créés avec Mermaid.

## 📊 Table des matières

1. [Diagramme de classes](#diagramme-de-classes)
2. [Diagrammes de séquence](#diagrammes-de-séquence)
3. [Diagramme de cas d'utilisation](#diagramme-de-cas-dutilisation)
4. [Diagramme d'états](#diagramme-détats)
5. [Diagramme de composants](#diagramme-de-composants)
6. [Diagramme de déploiement](#diagramme-de-déploiement)
7. [Diagramme d'activités](#diagramme-dactivités)

---

## 1. Diagramme de classes

### 1.1 Vue complète du modèle

```mermaid
classDiagram
    %% Interfaces
    class IState {
        <<interface>>
        +TimeSpan Time
        +Refresh() void
    }
    
    %% Models
    class ClockState {
        -TimeSpan _time
        +TimeSpan Time
        +Refresh() void
    }
    
    class CountdownState {
        -DateTime End
        -bool Alarm
        -TimeSpan _time
        +TimeSpan Time
        +DateTime End
        +bool Alarm
        +Refresh() void
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
    
    %% Relations
    IState <|.. ClockState : implements
    IState <|.. CountdownState : implements
    PomodoroSession --> ActivityType : uses
```

### 1.2 Services

```mermaid
classDiagram
    class DatabaseService {
        -SQLiteAsyncConnection? _connection
        -SemaphoreSlim _initSemaphore
        -bool _initialized
        -string DbPath$
        +InitializeAsync() Task
        -EnsureInitializedAsync() Task
        +SaveSessionAsync(PomodoroSession) Task~int~
        +GetSessionAsync(int) Task~PomodoroSession~
        +GetSessionsByDateAsync(DateTime) Task~List~PomodoroSession~~
        +GetSessionsAsync(DateTime, DateTime) Task~List~PomodoroSession~~
        +DeleteSessionAsync(int) Task
        +GetCompletedSessionsCountAsync(DateTime) Task~int~
        +GetTotalFocusMinutesAsync(DateTime) Task~int~
        +SaveJournalEntryAsync(JournalEntry) Task~int~
        +GetJournalEntryAsync(int) Task~JournalEntry~
        +GetJournalEntryByDateAsync(DateTime) Task~JournalEntry~
        +GetJournalEntriesAsync(DateTime, DateTime) Task~List~JournalEntry~~
        +DeleteJournalEntryAsync(int) Task
    }
    
    class TimerManager {
        -IState _state
        +IState State
        +PropertyChanged EventHandler
        +Pause() void
        +Play(TimeSpan) void
        +Stop() void
    }
    
    class PomodoroSessionService {
        -DatabaseService _databaseService
        -PomodoroSession? _currentSession
        +StartSession(ActivityType, int) void
        +CompleteSessionAsync(int) Task
        +GetCurrentSession() PomodoroSession?
    }
    
    class StatisticsService {
        -DatabaseService _databaseService
        +GetTodayStatisticsAsync() Task~DailyStats~
        +GetWeeklyStatisticsAsync() Task~List~DailyStats~~
        +GetMonthlyStatisticsAsync(int, int) Task~MonthlyStats~
        +GetCurrentStreakAsync() Task~int~
    }
    
    class SettingsService {
        +int SprintDuration
        +int ShortBreakDuration
        +int LongBreakDuration
        +bool LoopPlayback
        +string Mode
    }
    
    class ThemeService {
        +Initialize() void
        +SetTheme(AppTheme) void
        +ToggleTheme() void
    }
    
    %% Relations
    PomodoroSessionService --> DatabaseService : uses
    StatisticsService --> DatabaseService : uses
    TimerManager --> IState : uses
```

### 1.3 ViewModels

```mermaid
classDiagram
    class ObservableObject {
        <<abstract>>
        +PropertyChanged EventHandler
        #OnPropertyChanged(string) void
    }
    
    class MainViewModel {
        -TimerManager _timerManager
        -PomodoroSessionService _pomodoroSessionService
        -ActivityType _currentActivityType
        -bool _alarm
        -SettingsViewModel _settings
        -IState _state
        +bool Alarm
        +SettingsViewModel Settings
        +IState State
        +PauseCommand IRelayCommand
        +SetDurationCommand IRelayCommand~ActivityType~
        +StopCommand IRelayCommand
        +Pause() void
        +SetDuration(ActivityType) void
        +Stop() void
        -CanStop() bool
        -TimerManager_PropertyChanged(object, PropertyChangedEventArgs) void
    }
    
    class JournalViewModel {
        -DatabaseService _databaseService
        -DateTime _selectedDate
        -JournalEntry _currentEntry
        -ObservableCollection~JournalEntry~ _journalEntries
        -string _mood
        +DateTime SelectedDate
        +JournalEntry CurrentEntry
        +ObservableCollection~JournalEntry~ JournalEntries
        +string Mood
        +SaveJournalEntryCommand IAsyncRelayCommand
        +LoadJournalEntryCommand IAsyncRelayCommand
        +SetMoodCommand IAsyncRelayCommand~string~
        +SaveJournalEntry() Task
        +LoadJournalEntry() Task
        +SetMood(string) Task
        +LoadMonthlyView(int, int) Task
    }
    
    class StatisticsViewModel {
        -StatisticsService _statisticsService
        -int _todayPomodoros
        -int _todayFocusMinutes
        -int _currentStreak
        -string _streakMessage
        +int TodayPomodoros
        +int TodayFocusMinutes
        +int CurrentStreak
        +string StreakMessage
        +LoadTodayStatsCommand IAsyncRelayCommand
        +LoadTodayStats() Task
        +LoadMonthlyStats(int, int) Task
    }
    
    class SettingsViewModel {
        -SettingsService _settingsService
        -ThemeService _themeService
        -int _sprintDuration
        -int _shortBreakDuration
        -int _longBreakDuration
        -bool _loopPlayback
        -string _mode
        +int SprintDuration
        +int ShortBreakDuration
        +int LongBreakDuration
        +bool LoopPlayback
        +string Mode
        +ToggleThemeCommand IRelayCommand
        +ToggleTheme() void
    }
    
    %% Relations
    ObservableObject <|-- MainViewModel
    ObservableObject <|-- JournalViewModel
    ObservableObject <|-- StatisticsViewModel
    ObservableObject <|-- SettingsViewModel
    
    MainViewModel --> TimerManager : uses
    MainViewModel --> PomodoroSessionService : uses
    MainViewModel --> SettingsViewModel : contains
    JournalViewModel --> DatabaseService : uses
    StatisticsViewModel --> StatisticsService : uses
    SettingsViewModel --> SettingsService : uses
    SettingsViewModel --> ThemeService : uses
```

### 1.4 Contrôles et Drawables

```mermaid
classDiagram
    class ContentView {
        <<MAUI>>
        +BindableProperty
        +GetValue()
        +SetValue()
    }
    
    class CircularClock {
        -CircularDrawable _circularDrawable
        -IDispatcherTimer? _timer
        -bool _isDisposed
        +BindableProperty StateProperty$
        +BindableProperty TimeProperty$
        +IState? State
        +TimeSpan Time
        +EventHandler? Alarm
        -InitializeTimer() void
        -OnLoaded(object?, EventArgs) void
        -OnUnloaded(object?, EventArgs) void
        -OnStateChanged(BindableObject, object, object)$ void
        -OnTimeChanged(BindableObject, object, object)$ void
        -OnTimerTick(object?, EventArgs) void
        -UpdateTime(TimeSpan) void
        -UpdateDisplay(TimeSpan) void
        -OnAlarm() void
        #OnHandlerChanged() void
        -Dispose() void
    }
    
    class IDrawable {
        <<interface>>
        +Draw(ICanvas, RectF) void
    }
    
    class CircularDrawable {
        +Color SecondColor
        +Color MinuteColor
        +Color HourColor
        +float StrokeSize
        +double Hour
        +double Minute
        +double Second
        +bool ShowHours
        +Draw(ICanvas, RectF) void
        -DrawArcSegment(ICanvas, Color, float, float, float, float, int) void
    }
    
    %% Relations
    ContentView <|-- CircularClock
    IDrawable <|.. CircularDrawable
    CircularClock --> CircularDrawable : uses
    CircularClock --> IState : binds
```

### 1.5 Messages (Messaging Pattern)

```mermaid
classDiagram
    class AlarmMessage {
        +bool Value
        +AlarmMessage(bool)
    }
    
    class ResetAlarmMessage {
        +bool Value
        +ResetAlarmMessage(bool)
    }
    
    class StateUpdateMessage {
        +IState State
        +StateUpdateMessage(IState)
    }
    
    %% Ces classes héritent implicitement de ValueChangedMessage<T>
    %% mais on ne le montre pas pour simplifier
```

---

## 2. Diagrammes de séquence

### 2.1 Démarrage de l'application

```mermaid
sequenceDiagram
    actor User
    participant App as MauiApp
    participant MP as MauiProgram
    participant DI as DI Container
    participant DB as DatabaseService
    participant TM as TimerManager
    participant Shell as AppShell
    
    User->>App: Lance l'application
    App->>MP: CreateMauiApp()
    MP->>MP: SQLitePCL.Init()
    MP->>DI: RegisterServices()
    DI->>DI: AddSingleton(Services)
    DI->>DI: AddSingleton(ViewModels)
    DI->>DI: AddSingleton(Pages)
    MP->>App: Build()
    App->>Shell: NavigateTo(MainPage)
    Shell->>DI: Resolve(MainPage)
    DI->>DI: Resolve(MainViewModel)
    DI->>DI: Resolve(TimerManager)
    DI-->>TM: new TimerManager()
    TM->>TM: State = new ClockState()
    DI-->>Shell: MainPage créée
    Shell-->>User: Affiche MainPage
    
    Note over User,Shell: L'horloge démarre automatiquement<br/>en mode ClockState
```

### 2.2 Session Pomodoro complète

```mermaid
sequenceDiagram
    actor User
    participant V as MainPage
    participant VM as MainViewModel
    participant PS as PomodoroSessionService
    participant TM as TimerManager
    participant DB as DatabaseService
    participant CC as CircularClock
    participant M as Messenger
    
    User->>V: Clique "🍅 Pomodoro"
    V->>VM: SetDurationCommand(Pomodoro)
    VM->>M: Send(ResetAlarmMessage)
    M->>V: Stop MediaElement
    VM->>PS: StartSession(Pomodoro, 25)
    PS->>DB: SaveSessionAsync(session)
    DB->>DB: EnsureInitialized()
    DB->>DB: InsertAsync(session)
    DB-->>PS: Session Id
    PS-->>VM: Session démarrée
    VM->>TM: Play(TimeSpan.FromMinutes(25))
    TM->>TM: State = new CountdownState(25min)
    TM-->>VM: PropertyChanged(State)
    VM-->>CC: State updated (binding)
    CC->>CC: Start Timer
    
    loop Every 100ms
        CC->>CC: OnTimerTick()
        CC->>TM: State.Refresh()
        TM-->>CC: Time updated
        CC->>CC: UpdateDisplay()
    end
    
    alt Timer atteint 0
        TM->>M: Send(AlarmMessage)
        M->>V: Play MediaElement
        CC->>VM: Alarm event
    end
    
    User->>V: Clique "⏹️ Stop"
    V->>VM: StopCommand
    VM->>PS: CompleteSessionAsync(25)
    PS->>DB: UpdateSessionAsync(session)
    DB-->>PS: Session mise à jour
    VM->>TM: Stop()
    TM->>TM: State = new ClockState()
    TM-->>VM: PropertyChanged(State)
    VM-->>CC: State updated
    CC->>CC: Affiche l'heure
```

### 2.3 Sauvegarde d'entrée de journal

```mermaid
sequenceDiagram
    actor User
    participant V as JournalPage
    participant VM as JournalViewModel
    participant DB as DatabaseService
    participant SQLite as SQLite DB
    
    User->>V: OnAppearing()
    V->>VM: LoadJournalEntryCommand
    VM->>DB: GetJournalEntryByDateAsync(Today)
    DB->>DB: EnsureInitializedAsync()
    DB->>SQLite: SELECT * WHERE Date = ?
    SQLite-->>DB: JournalEntry or null
    
    alt Entry existe
        DB-->>VM: JournalEntry
        VM->>VM: CurrentEntry = entry
        VM-->>V: Update UI
    else Nouvelle entrée
        DB-->>VM: null
        VM->>VM: CurrentEntry = new JournalEntry()
        VM-->>V: UI vide
    end
    
    User->>V: Saisit du texte
    V-->>VM: Binding Title, Content
    
    User->>V: Change Mood "😊"
    V->>VM: SetMoodCommand("😊")
    VM->>VM: Mood = "😊"
    
    User->>V: Clique Save
    V->>VM: SaveJournalEntryCommand
    
    VM->>VM: entry.UpdatedAt = Now
    VM->>DB: SaveJournalEntryAsync(entry)
    DB->>DB: EnsureInitializedAsync()
    
    alt Nouvelle entrée
        DB->>SQLite: INSERT INTO journal_entries
        SQLite-->>DB: New Id
    else Mise à jour
        DB->>SQLite: UPDATE journal_entries WHERE Id = ?
        SQLite-->>DB: Rows affected
    end
    
    DB-->>VM: Success
    VM-->>V: Notification
    V-->>User: Toast "Saved ✓"
```

### 2.4 Chargement des statistiques

```mermaid
sequenceDiagram
    actor User
    participant V as StatisticsPage
    participant VM as StatisticsViewModel
    participant SS as StatisticsService
    participant DB as DatabaseService
    
    User->>V: Navigate to Statistics
    V->>V: OnAppearing()
    V->>VM: LoadTodayStatsCommand
    VM->>SS: GetTodayStatisticsAsync()
    SS->>DB: GetSessionsByDateAsync(Today)
    DB-->>SS: List<PomodoroSession>
    
    SS->>SS: Calculate Pomodoros
    SS->>SS: Calculate TotalMinutes
    SS-->>VM: DailyStats
    
    VM->>VM: TodayPomodoros = stats.Pomodoros
    VM->>VM: TodayFocusMinutes = stats.Minutes
    
    VM->>SS: GetCurrentStreakAsync()
    
    loop For each previous day
        SS->>DB: GetCompletedSessionsCountAsync(date)
        DB-->>SS: count
        
        alt count > 0
            SS->>SS: streak++
        else count = 0
            SS->>SS: break loop
        end
    end
    
    SS-->>VM: streak count
    VM->>VM: CurrentStreak = streak
    VM->>VM: UpdateStreakMessage()
    VM-->>V: Update UI bindings
    V-->>User: Affiche stats
```

---

## 3. Diagramme de cas d'utilisation

```mermaid
graph TB
    subgraph "Système MReveil"
        UC1[Démarrer un Pomodoro]
        UC2[Mettre en pause le timer]
        UC3[Arrêter le timer]
        UC4[Voir l'heure actuelle]
        UC5[Écrire une entrée de journal]
        UC6[Consulter les statistiques]
        UC7[Modifier les paramètres]
        UC8[Changer le thème]
        UC9[Consulter l'historique]
    end
    
    User((Utilisateur))
    
    User --> UC1
    User --> UC2
    User --> UC3
    User --> UC4
    User --> UC5
    User --> UC6
    User --> UC7
    User --> UC8
    User --> UC9
    
    UC1 -.-> UC3 : extends
    UC2 -.-> UC1 : requires
    UC5 -.-> UC6 : updates
    UC1 -.-> UC9 : creates entry
```

---

## 4. Diagramme d'états

### 4.1 États du Timer

```mermaid
stateDiagram-v2
    [*] --> ClockState : Application démarre
    
    ClockState --> CountdownState : Play(duration)
    CountdownState --> ClockState : Stop()
    CountdownState --> PausedState : Pause()
    PausedState --> CountdownState : Resume()
    PausedState --> ClockState : Stop()
    
    state CountdownState {
        [*] --> Running
        Running --> Alarming : Time <= 0
        Alarming --> [*]
    }
    
    state ClockState {
        [*] --> ShowingTime
        ShowingTime --> ShowingTime : Refresh() every 100ms
    }
    
    note right of ClockState
        Affiche l'heure actuelle
        avec heures, minutes, secondes
    end note
    
    note right of CountdownState
        Compte à rebours
        affiche minutes:secondes
    end note
```

### 4.2 États de la Session Pomodoro

```mermaid
stateDiagram-v2
    [*] --> NotStarted
    
    NotStarted --> Active : StartSession()
    Active --> Completed : CompleteSessionAsync()
    Active --> Abandoned : Stop() sans complétion
    Completed --> [*]
    Abandoned --> [*]
    
    state Active {
        [*] --> Running
        Running --> Paused : Pause()
        Paused --> Running : Resume()
        Running --> TimeExpired : Timer = 0
        TimeExpired --> [*]
    }
    
    note right of Completed
        Session enregistrée avec:
        - ActualDuration
        - CompletedAt
        - IsCompleted = true
    end note
```

---

## 5. Diagramme de composants

```mermaid
graph TB
    subgraph "MReveil.App"
        subgraph "Presentation Layer"
            V[Views]
            C[Controls]
        end
        
        subgraph "Application Layer"
            VM[ViewModels]
            M[Messaging]
        end
        
        subgraph "Business Layer"
            S[Services]
        end
        
        subgraph "Data Layer"
            MD[Models]
            DS[DatabaseService]
        end
    end
    
    subgraph "External"
        MAUI[.NET MAUI]
        CT[CommunityToolkit]
        SQLite[SQLite]
    end
    
    V --> VM
    C --> VM
    VM --> M
    VM --> S
    S --> DS
    DS --> MD
    DS --> SQLite
    
    V --> MAUI
    C --> MAUI
    VM --> CT
    M --> CT
```

---

## 6. Diagramme de déploiement

```mermaid
graph TB
    subgraph "Dispositif utilisateur"
        subgraph "Windows"
            WApp[MReveil.exe]
            WDB[(SQLite DB)]
            WPrefs[Preferences]
        end
        
        subgraph "Android"
            AApp[MReveil.apk]
            ADB[(SQLite DB)]
            APrefs[SharedPreferences]
        end
        
        subgraph "iOS"
            IApp[MReveil.app]
            IDB[(SQLite DB)]
            IPrefs[UserDefaults]
        end
        
        subgraph "macOS"
            MApp[MReveil.app]
            MDB[(SQLite DB)]
            MPrefs[UserDefaults]
        end
    end
    
    WApp --> WDB
    WApp --> WPrefs
    AApp --> ADB
    AApp --> APrefs
    IApp --> IDB
    IApp --> IPrefs
    MApp --> MDB
    MApp --> MPrefs
    
    note1[Base de données locale<br/>Pas de serveur requis]
    
    style note1 fill:#ffffcc
```

---

## 7. Diagramme d'activités

### 7.1 Processus de session Pomodoro

```mermaid
flowchart TD
    Start([Utilisateur arrive sur MainPage]) --> ShowClock[Afficher l'horloge]
    ShowClock --> SelectMode{Sélection du mode}
    
    SelectMode -->|Pomodoro| SetPomodoro[Durée = 25 min]
    SelectMode -->|Short Break| SetShort[Durée = 5 min]
    SelectMode -->|Long Break| SetLong[Durée = 15 min]
    
    SetPomodoro --> CreateSession[Créer PomodoroSession]
    SetShort --> CreateSession
    SetLong --> CreateSession
    
    CreateSession --> SaveDB[(Sauvegarder en DB)]
    SaveDB --> StartTimer[Démarrer CountdownState]
    StartTimer --> UpdateLoop{Timer actif?}
    
    UpdateLoop -->|Oui| CheckTime{Time > 0?}
    CheckTime -->|Oui| Refresh[Rafraîchir affichage]
    Refresh --> UpdateLoop
    
    CheckTime -->|Non| TriggerAlarm[Déclencher alarme]
    TriggerAlarm --> PlaySound[Jouer le son]
    
    UpdateLoop -->|Stop cliqué| UserStop{Session complétée?}
    UserStop -->|Oui| Complete[CompleteSessionAsync]
    UserStop -->|Non| Abandon[Abandonner session]
    
    Complete --> UpdateSession[(Mettre à jour DB)]
    UpdateSession --> ReturnClock[Retour ClockState]
    Abandon --> ReturnClock
    PlaySound --> WaitStop[Attendre action utilisateur]
    WaitStop --> Complete
    
    ReturnClock --> End([Fin])
```

### 7.2 Sauvegarde automatique du journal

```mermaid
flowchart TD
    Start([Édition du journal]) --> TypeText[Utilisateur tape du texte]
    TypeText --> Debounce{Délai écoulé?}
    
    Debounce -->|Non| TypeText
    Debounce -->|Oui| PrepareEntry[Préparer JournalEntry]
    
    PrepareEntry --> SetDate[Date = SelectedDate]
    SetDate --> SetContent[Content = texte saisi]
    SetContent --> SetStats[Stats du jour]
    SetStats --> SetMood[Mood = emoji sélectionné]
    
    SetMood --> CheckExists{Entrée existe?}
    
    CheckExists -->|Oui| UpdateEntry[UpdatedAt = Now]
    CheckExists -->|Non| CreateEntry[CreatedAt = Now]
    
    UpdateEntry --> SaveDB[(Sauvegarder DB)]
    CreateEntry --> SaveDB
    
    SaveDB --> Success{Succès?}
    
    Success -->|Oui| ShowToast[Afficher Toast "Saved"]
    Success -->|Non| ShowError[Afficher erreur]
    
    ShowToast --> End([Fin])
    ShowError --> End
```

---

## 📝 Notes sur les diagrammes

### Conventions utilisées

- **Classes abstraites** : Notation `<<abstract>>`
- **Interfaces** : Notation `<<interface>>`
- **Énumérations** : Notation `<<enumeration>>`
- **Membres statiques** : Suffixe `$`
- **Membres privés** : Préfixe `-`
- **Membres publics** : Préfixe `+`
- **Membres protégés** : Préfixe `#`

### Relations

- `-->` : Dépendance
- `<|--` : Héritage
- `<|..` : Implémentation d'interface
- `*-->` : Composition
- `o-->` : Agrégation

### Stéréotypes personnalisés

- `<<MAUI>>` : Composants du framework MAUI
- `<<Database>>` : Entités de base de données
- `<<Service>>` : Services métier
