# 🏗️ MReveil Build System (Nuke)

Système de build automatisé pour MReveil utilisant [Nuke Build](https://nuke.build/).

## 📋 Prérequis

### Pour tous les builds
- .NET 10 SDK
- Visual Studio 2022+ ou JetBrains Rider

### Pour macOS (DMG)
- macOS avec Xcode installé
- Command Line Tools : `xcode-select --install`

### Pour Windows (MSIX)
- Windows 10/11
- Windows SDK 10.0.19041.0+

### Pour Android (APK/AAB)
- Android SDK
- JDK 17+

### Pour iOS
- macOS avec Xcode
- Apple Developer Account (pour signing)

---

## 🚀 Utilisation

### Commandes de base

```bash
# Afficher tous les targets disponibles
./build --help

# Build simple (compilation uniquement)
./build Compile

# Clean + Build
./build Clean Compile
```

### Build par plateforme

#### 🍎 macOS

```bash
# Build macOS Catalyst
./build PublishMacOS

# Créer un DMG (macOS uniquement)
./build CreateDMG

# Avec version personnalisée
./build CreateDMG --version 2.0.0
```

**Sortie :** `artifacts/MReveil-1.0.0.dmg`

#### 🪟 Windows

```bash
# Build Windows
./build PublishWindows

# Créer un package MSIX (Windows uniquement)
./build CreateMSIX
```

**Sortie :** `artifacts/windows/MReveil.exe`

#### 🤖 Android

```bash
# Créer un APK
./build PublishAndroid

# Créer un AAB (pour Google Play)
./build PublishAndroidAAB
```

**Sortie :** 
- APK : `artifacts/android/com.monbsoft.mreveil-Signed.apk`
- AAB : `artifacts/android/aab/com.monbsoft.mreveil.aab`

#### 📱 iOS

```bash
# Build iOS (macOS uniquement)
./build PublishIOS
```

**Sortie :** `artifacts/ios/MReveil.app`

### Build multi-plateforme

```bash
# Build toutes les plateformes (sauf iOS)
./build BuildAll

# Spécifier la configuration
./build BuildAll --configuration Release
```

---

## 📁 Structure des artifacts

```
artifacts/
├── macos/
│   └── MReveil.app              # Application macOS
├── windows/
│   ├── MReveil.exe              # Exécutable Windows
│   └── msix/                    # Package MSIX
├── android/
│   ├── com.monbsoft.mreveil.apk # APK Android
│   └── aab/
│       └── com.monbsoft.mreveil.aab # Bundle Google Play
├── ios/
│   └── MReveil.app              # Application iOS
└── MReveil-1.0.0.dmg            # Image disque macOS
```

---

## 🔧 Configuration avancée

### Paramètres disponibles

| Paramètre | Description | Défaut | Exemple |
|-----------|-------------|--------|---------|
| `--configuration` | Debug ou Release | `Debug` (local) / `Release` (CI) | `--configuration Release` |
| `--version` | Version de l'app | `1.0.0` | `--version 2.1.3` |

### Exemples

```bash
# Build Release avec version spécifique
./build CreateDMG --configuration Release --version 1.2.0

# Build Android signé pour production
./build PublishAndroidAAB --configuration Release

# Clean complet puis build toutes les plateformes
./build Clean BuildAll --configuration Release
```

---

## 🎯 Targets disponibles

### Targets de base
- **Clean** : Nettoie les répertoires bin/obj et artifacts
- **Restore** : Restore les packages NuGet
- **Compile** : Compile la solution
- **Publish** : Publie l'application générique

### Targets macOS
- **PublishMacOS** : Build pour macOS Catalyst (ARM64)
- **CreateDMG** : Crée une image disque .dmg

### Targets Windows
- **PublishWindows** : Build pour Windows (x64)
- **CreateMSIX** : Crée un package MSIX

### Targets Android
- **PublishAndroid** : Crée un APK
- **PublishAndroidAAB** : Crée un App Bundle (AAB)

### Targets iOS
- **PublishIOS** : Build pour iOS (ARM64)

### Targets combinés
- **BuildAll** : Build macOS + Windows + Android

---

## 🐛 Troubleshooting

### Erreur : "hdiutil: command not found"
**Solution :** Vous n'êtes pas sur macOS. Le target `CreateDMG` nécessite macOS.

### Erreur : "No matching project found"
**Solution :** Assurez-vous d'être dans le répertoire racine du projet.

### Erreur : Android SDK introuvable
**Solution :** 
1. Installer Android SDK via Visual Studio Installer
2. Définir `ANDROID_HOME` :
   ```bash
   export ANDROID_HOME=$HOME/Library/Android/sdk  # macOS
   set ANDROID_HOME=C:\Android\sdk                # Windows
   ```

### Erreur : "Code signing required" (iOS/macOS)
**Solution :** 
1. Installer Xcode
2. Ouvrir Xcode → Preferences → Accounts → Ajouter Apple ID
3. Signer manuellement après le build

---

## 📝 Scripts de build (.sh / .ps1)

Les anciens scripts sont toujours disponibles mais **obsolètes**. Utilisez Nuke à la place :

| Ancien script | Équivalent Nuke |
|--------------|----------------|
| `build-macos.sh` | `./build CreateDMG` |
| `build-macos-advanced.sh` | `./build CreateDMG` |
| `make dmg` | `./build CreateDMG` |

---

## 🚀 CI/CD Integration

### GitHub Actions

```yaml
name: Build

on: [push, pull_request]

jobs:
  build-macos:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Build DMG
        run: ./build CreateDMG --configuration Release

  build-windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Build Windows
        run: .\build.cmd PublishWindows --configuration Release

  build-android:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Build Android
        run: ./build PublishAndroid --configuration Release
```

---

## 📚 Ressources

- [Nuke Build Documentation](https://nuke.build/)
- [.NET MAUI Publishing](https://learn.microsoft.com/en-us/dotnet/maui/deployment/)
- [macOS App Distribution](https://developer.apple.com/macos/distribution/)
- [Windows MSIX Packaging](https://learn.microsoft.com/en-us/windows/msix/)
- [Android App Publishing](https://developer.android.com/studio/publish)

---

## 🎉 Contribution

Pour ajouter de nouveaux targets :

1. Éditer `build/Build.cs`
2. Ajouter un nouveau target :
   ```csharp
   Target MyNewTarget => _ => _
       .DependsOn(Clean)
       .Executes(() =>
       {
           Serilog.Log.Information("🚀 My new target");
           // Your code here
       });
   ```
3. Tester : `./build MyNewTarget`

---

**Créé avec ❤️ pour MReveil**
