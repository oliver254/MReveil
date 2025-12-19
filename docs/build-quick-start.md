# 🚀 Guide de démarrage rapide - Build System

## Commandes essentielles

### 🍎 macOS - Créer un DMG

```bash
# Sur macOS
./build.sh CreateDMG

# Avec version spécifique
./build.sh CreateDMG --version 2.0.0 --configuration Release
```

**Résultat :** `artifacts/MReveil-1.0.0.dmg`

---

### 🪟 Windows - Créer un package

```powershell
# Sur Windows
.\build.cmd PublishWindows

# Créer MSIX
.\build.cmd CreateMSIX --configuration Release
```

**Résultat :** `artifacts\windows\MReveil.exe`

---

### 🤖 Android - Créer APK/AAB

```bash
# APK (pour sideloading)
./build.sh PublishAndroid

# AAB (pour Google Play)
./build.sh PublishAndroidAAB --configuration Release
```

**Résultat :** 
- `artifacts/android/com.monbsoft.mreveil.apk`
- `artifacts/android/aab/com.monbsoft.mreveil.aab`

---

### 🌍 Build toutes les plateformes

```bash
# Linux/macOS
./build.sh BuildAll --configuration Release

# Windows
.\build.cmd BuildAll --configuration Release
```

---

## 📦 Distribution

### macOS
1. Créer le DMG : `./build.sh CreateDMG --configuration Release`
2. Signer (optionnel) : `codesign -s "Developer ID" artifacts/MReveil-1.0.0.dmg`
3. Notariser pour Gatekeeper (si distribution hors App Store)
4. Distribuer le DMG

### Windows
1. Build : `.\build.cmd PublishWindows --configuration Release`
2. Créer un installateur (Inno Setup, WiX, ou MSIX)
3. Signer le package
4. Distribuer via Microsoft Store ou site web

### Android
1. AAB : `./build.sh PublishAndroidAAB --configuration Release`
2. Signer avec keystore
3. Uploader sur Google Play Console

---

## ⚡ Raccourcis

```bash
# Nettoyage complet
./build.sh Clean

# Compilation simple
./build.sh Compile

# Voir tous les targets
./build.sh --help
```

---

## 🐛 Dépannage rapide

### "Command not found: build.sh"
```bash
chmod +x build.sh
./build.sh
```

### "hdiutil: command not found"
Vous n'êtes pas sur macOS. Utilisez un Mac pour créer des DMG.

### "Android SDK not found"
1. Installer via Visual Studio
2. Définir : `export ANDROID_HOME=/path/to/android/sdk`

---

Voir [build-system.md](./build-system.md) pour la documentation complète.
