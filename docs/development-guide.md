# Guide de développement - MReveil

## 🚀 Configuration de l'environnement de développement

### Prérequis

#### Logiciels requis
- **Visual Studio 2022** (version 17.12 ou supérieure)
  - Charge de travail : Développement multiplateforme .NET
  - Charge de travail : Développement .NET Desktop (pour Windows)
- **.NET 10 SDK** (10.0.101 ou supérieur)
- **Git** pour le contrôle de version

#### Plateformes cibles spécifiques

**Pour Windows:**
- Windows 10 version 1809 ou supérieure
- SDK Windows 10.0.19041.0

**Pour Android:**
- Android SDK (API 24+)
- Émulateur Android ou appareil physique
- JDK 17

**Pour iOS/macOS:**
- macOS Monterey (12.0) ou supérieur
- Xcode 14.0 ou supérieur
- Compte développeur Apple (pour le déploiement)

### Installation

#### 1. Cloner le repository

```bash
git clone https://github.com/oliver254/MReveil.git
cd MReveil
```

#### 2. Restaurer les dépendances

```bash
dotnet restore src/MReveil/MReveil.csproj
```

#### 3. Vérifier l'installation

```bash
dotnet build src/MReveil/MReveil.csproj
```

Si tout est correct, la build devrait réussir sans erreurs.

### Configuration de Visual Studio

#### Extensions recommandées
- **XAML Styler** - Formatage automatique XAML
- **Productivity Power Tools** - Améliorations de productivité
- **GitHub Copilot** - Assistance IA (optionnel)
- **ReSharper** - Refactoring et analyse de code (optionnel)

#### Paramètres recommandés

**Éditeur:**
- Tabulations : 4 espaces
- Encodage : UTF-8
- Fin de ligne : CRLF (Windows) ou LF (Mac/Linux)
- Trim trailing whitespace : Activé

**Code Analysis:**
- Activer l'analyse de code .NET
- Niveau de diagnostic : Warning
- Treat warnings as errors : Non (en développement)

## 📁 Structure du projet

### Organisation des fichiers

```
src/MReveil/
├── Controls/              # Contrôles UI personnalisés
│   ├── CircularClock.xaml
│   └── CircularClock.xaml.cs
├── Drawables/            # Rendus graphiques personnalisés
│   └── CircularDrawable.cs
├── Messaging/            # Messages pour communication inter-composants
│   ├── AlarmMessage.cs
│   ├── ResetAlarmMessage.cs
│   └── StateUpdateMessage.cs
├── Models/               # Modèles de données et états
│   ├── ActivityType.cs
│   ├── ClockState.cs
│   ├── CountdownState.cs
│   ├── IState.cs
│   ├── JournalEntry.cs
│   └── PomodoroSession.cs
├── Platforms/            # Code spécifique aux plateformes
│   ├── Android/
│   ├── iOS/
│   ├── MacCatalyst/
│   └── Windows/
├── Resources/            # Ressources de l'application
│   ├── Fonts/
│   ├── Images/
│   ├── Raw/
│   └── Styles/
├── Services/             # Services métier
│   ├── DatabaseService.cs
│   ├── PomodoroSessionService.cs
│   ├── SettingsService.cs
│   ├── StatisticsService.cs
│   ├── ThemeService.cs
│   └── TimerManager.cs
├── ViewModels/           # ViewModels MVVM
│   ├── JournalViewModel.cs
│   ├── MainViewModel.cs
│   ├── SettingsViewModel.cs
│   └── StatisticsViewModel.cs
├── Views/                # Pages et vues XAML
│   ├── JournalPage.xaml[.cs]
│   ├── MainPage.xaml[.cs]
│   ├── SettingsPage.xaml[.cs]
│   └── StatisticsPage.xaml[.cs]
├── App.xaml[.cs]         # Application principale
├── AppShell.xaml[.cs]    # Shell de navigation
└── MauiProgram.cs        # Point d'entrée et configuration DI
```

### Conventions de nommage

Le projet suit les conventions de nommage C# standard avec quelques règles supplémentaires :

#### Fichiers et classes
- **PascalCase** pour tous les noms de classes, interfaces, méthodes, propriétés
- **camelCase** pour les variables locales et paramètres
- **_camelCase** (avec underscore) pour les champs privés

#### Exemples
```csharp
// ✅ Bon
public class DatabaseService { }
private readonly TimerManager _timerManager;
public int TodayPomodoros { get; set; }
private async Task SaveSessionAsync() { }

// ❌ Mauvais
public class databaseService { }
private readonly TimerManager timerManager;
public int todayPomodoros { get; set; }
private async Task save_session() { }
```

#### XAML
- **PascalCase** pour les noms de contrôles (`x:Name`)
- **PascalCase** pour les propriétés et événements

```xml
<!-- ✅ Bon -->
<Button x:Name="StartButton" 
        Text="Start"
        Command="{Binding StartCommand}" />

<!-- ❌ Mauvais -->
<Button x:Name="start_button" 
        Text="Start"
        Command="{Binding startCommand}" />
```

## 🏗️ Patterns et principes

### Architecture MVVM

L'application suit strictement le pattern MVVM. Voici les responsabilités de chaque couche :

#### View (XAML + Code-behind)
**Responsabilités:**
- Définir l'interface utilisateur
- Binding vers le ViewModel
- Gestion des événements UI de base (Loaded, Unloaded)

**Ne PAS faire:**
- Logique métier
- Accès direct aux services
- Manipulation des données

**Exemple:**
```csharp
public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // ✅ OK - Gestion du cycle de vie
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Inscription aux messages
    }

    // ❌ PAS OK - Logique métier dans la vue
    private void SaveData()
    {
        var data = GetDataFromUI();
        _database.Save(data);
    }
}
```

#### ViewModel
**Responsabilités:**
- Logique de présentation
- Propriétés bindables
- Commandes
- Validation
- Communication avec les services

**Ne PAS faire:**
- Référencer des contrôles UI
- Logique de base de données directe

**Exemple:**
```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly TimerManager _timerManager;
    
    [ObservableProperty]
    private IState _state;

    public MainViewModel(TimerManager timerManager)
    {
        _timerManager = timerManager;
        _state = _timerManager.State;
    }

    [RelayCommand]
    public void SetDuration(ActivityType type)
    {
        // ✅ OK - Logique de présentation
        int duration = type switch
        {
            ActivityType.Pomodoro => 25,
            ActivityType.ShortBreak => 5,
            ActivityType.LongBreak => 15,
            _ => 25
        };
        
        _timerManager.Play(TimeSpan.FromMinutes(duration));
    }
}
```

#### Model
**Responsabilités:**
- Représenter les données
- Règles métier de base
- Validation des données

**Exemple:**
```csharp
[Table("pomodoro_sessions")]
public class PomodoroSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public DateTime Date { get; set; }

    public ActivityType Type { get; set; }

    public int PlannedDuration { get; set; }

    public int ActualDuration { get; set; }

    public bool IsCompleted { get; set; }

    // ✅ OK - Logique métier simple
    public bool WasFullyCompleted => 
        IsCompleted && ActualDuration >= PlannedDuration;
}
```

#### Services
**Responsabilités:**
- Logique métier complexe
- Accès aux données
- Opérations asynchrones
- Communication externe

**Exemple:**
```csharp
public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly SemaphoreSlim _initSemaphore = new(1, 1);
    
    // ✅ OK - Initialisation thread-safe
    private async Task EnsureInitializedAsync()
    {
        if (!_initialized)
        {
            await InitializeAsync();
        }
    }

    // ✅ OK - Opération asynchrone
    public async Task<List<PomodoroSession>> GetSessionsByDateAsync(DateTime date)
    {
        await EnsureInitializedAsync();
        
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _connection!.Table<PomodoroSession>()
            .Where(s => s.Date >= startOfDay && s.Date < endOfDay)
            .ToListAsync();
    }
}
```

### Injection de dépendances

Toutes les dépendances sont configurées dans `MauiProgram.cs` :

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        
        // ✅ Enregistrement des services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<TimerManager>();
        builder.Services.AddSingleton<SettingsService>();
        
        // ✅ Enregistrement des ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<JournalViewModel>();
        
        // ✅ Enregistrement des Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<JournalPage>();

        return builder.Build();
    }
}
```

**Règles:**
1. Toujours injecter les dépendances via le constructeur
2. Utiliser des interfaces quand approprié
3. Préférer `AddSingleton` pour les services sans état
4. Utiliser `AddTransient` pour les services avec état

### Programmation asynchrone

#### Bonnes pratiques

```csharp
// ✅ Bon - Async/await tout le chemin
public async Task LoadDataAsync()
{
    var sessions = await _database.GetSessionsAsync();
    Sessions = new ObservableCollection<Session>(sessions);
}

// ❌ Mauvais - Bloquer avec .Result
public void LoadData()
{
    var sessions = _database.GetSessionsAsync().Result;
    Sessions = new ObservableCollection<Session>(sessions);
}

// ✅ Bon - Gestion des erreurs
public async Task SaveAsync()
{
    try
    {
        await _database.SaveAsync(data);
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error saving: {ex.Message}");
        // Informer l'utilisateur
    }
}
```

#### Éviter les deadlocks

```csharp
// ✅ Bon - Initialisation asynchrone non-bloquante
public MauiProgram()
{
    var app = builder.Build();
    
    // Initialisation en arrière-plan
    Task.Run(async () =>
    {
        var db = app.Services.GetRequiredService<DatabaseService>();
        await db.InitializeAsync();
    });
    
    return app;
}

// ❌ Mauvais - Blocage du thread UI
public MauiProgram()
{
    var app = builder.Build();
    var db = app.Services.GetRequiredService<DatabaseService>();
    db.InitializeAsync().Wait(); // Peut causer un deadlock
    return app;
}
```

### Gestion des ressources

#### IDisposable

```csharp
public class CircularClock : ContentView
{
    private IDispatcherTimer? _timer;
    private bool _isDisposed;

    // ✅ Bon - Nettoyage approprié
    private void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }

        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler == null)
        {
            Dispose();
        }
    }
}
```

#### WeakReferenceMessenger

```csharp
// ✅ Bon - Inscription et désinscription
protected override void OnAppearing()
{
    base.OnAppearing();
    WeakReferenceMessenger.Default.Register<AlarmMessage>(this, OnAlarmReceived);
}

protected override void OnDisappearing()
{
    base.OnDisappearing();
    WeakReferenceMessenger.Default.Unregister<AlarmMessage>(this);
}

// ❌ Mauvais - Oublier de se désinscrire
protected override void OnAppearing()
{
    WeakReferenceMessenger.Default.Register<AlarmMessage>(this, OnAlarmReceived);
    // Pas de désinscription = fuite mémoire potentielle
}
```

## 🧪 Tests

### Tests unitaires

#### Structure des tests

```csharp
public class DatabaseServiceTests
{
    private DatabaseService _sut; // System Under Test
    private string _testDbPath;

    [SetUp]
    public async Task Setup()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _sut = new DatabaseService(_testDbPath);
        await _sut.InitializeAsync();
    }

    [TearDown]
    public void Cleanup()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    [Test]
    public async Task SaveSessionAsync_WhenCalled_ShouldSaveToDatabase()
    {
        // Arrange
        var session = new PomodoroSession
        {
            Date = DateTime.Now,
            Type = ActivityType.Pomodoro,
            PlannedDuration = 25
        };

        // Act
        var id = await _sut.SaveSessionAsync(session);

        // Assert
        Assert.That(id, Is.GreaterThan(0));
        var savedSession = await _sut.GetSessionAsync(id);
        Assert.That(savedSession.Type, Is.EqualTo(ActivityType.Pomodoro));
    }
}
```

### Tests d'intégration

```csharp
[TestFixture]
public class PomodoroWorkflowTests
{
    [Test]
    public async Task CompletePomodoroCycle_ShouldUpdateStatistics()
    {
        // Arrange
        var db = new DatabaseService();
        var sessionService = new PomodoroSessionService(db);
        var statsService = new StatisticsService(db);

        // Act
        sessionService.StartSession(ActivityType.Pomodoro, 25);
        await sessionService.CompleteSessionAsync(25);

        var stats = await statsService.GetTodayStatisticsAsync();

        // Assert
        Assert.That(stats.PomodorosCompleted, Is.EqualTo(1));
        Assert.That(stats.TotalFocusMinutes, Is.EqualTo(25));
    }
}
```

## 🐛 Débogage

### Logging

```csharp
// Utiliser Debug.WriteLine pour le logging
Debug.WriteLine($"Session started: {sessionId}");
Debug.WriteLine($"Error in timer: {ex.Message}");
```

### Points d'arrêt conditionnels

Dans Visual Studio, clic droit sur un point d'arrêt > Conditions...

```csharp
// S'arrêter seulement si Type == Pomodoro
public async Task SaveSessionAsync(PomodoroSession session)
{
    // Condition: session.Type == ActivityType.Pomodoro
    await _database.InsertAsync(session);
}
```

### Outils de débogage

1. **Live Visual Tree** - Inspecter la hiérarchie UI
2. **Hot Reload** - Modifications XAML en temps réel
3. **Diagnostic Tools** - Profiling CPU et mémoire
4. **SQLite Browser** - Inspecter la base de données

## 📝 Documentation

### XML Comments

Tous les membres publics doivent avoir des commentaires XML :

```csharp
/// <summary>
/// Démarre une nouvelle session Pomodoro.
/// </summary>
/// <param name="type">Le type d'activité (Pomodoro, pause courte, pause longue).</param>
/// <param name="duration">La durée prévue en minutes.</param>
/// <exception cref="InvalidOperationException">
/// Lancée si une session est déjà en cours.
/// </exception>
public void StartSession(ActivityType type, int duration)
{
    // ...
}
```

### README dans les dossiers

Chaque dossier important devrait avoir un README.md expliquant son contenu.

## 🚀 Déploiement

### Build de release

#### Windows

```bash
dotnet publish src/MReveil/MReveil.csproj -f net10.0-windows10.0.19041.0 -c Release
```

#### Android

```bash
dotnet publish src/MReveil/MReveil.csproj -f net10.0-android -c Release
```

#### iOS

```bash
dotnet publish src/MReveil/MReveil.csproj -f net10.0-ios -c Release
```

### Checklist avant release

- [ ] Incrémenter le numéro de version dans .csproj
- [ ] Mettre à jour CHANGELOG.md
- [ ] Exécuter tous les tests
- [ ] Tester sur toutes les plateformes cibles
- [ ] Vérifier les icônes et splash screens
- [ ] Vérifier les permissions requises
- [ ] Générer les builds signés

## 🔧 Dépannage

### Problèmes courants

#### La base de données ne s'initialise pas

**Symptôme:** Exception au démarrage  
**Solution:** Vérifier que `SQLitePCL.Batteries_V2.Init()` est appelé dans `MauiProgram.cs`

#### Le timer ne démarre pas

**Symptôme:** L'horloge reste figée  
**Solution:** Vérifier que le State n'est pas null et que le timer est démarré dans OnLoaded

#### Fuites mémoire

**Symptôme:** Consommation mémoire croissante  
**Solution:** Vérifier la désinscription des événements et messages

## 📚 Ressources supplémentaires

- [Documentation .NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [SQLite-net](https://github.com/praeclarum/sqlite-net)
- [Material Design Icons](https://materialdesignicons.com/)

---

**Dernière mise à jour:** 15 Janvier 2024  
**Maintenu par:** Oliver254
