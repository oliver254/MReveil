# Test du Build System

## 🧪 Tests rapides

### 1. Vérifier que Nuke fonctionne

```bash
# Linux/macOS
./build.sh --help

# Windows
.\build.cmd --help
```

**Attendu :** Liste de tous les targets disponibles

---

### 2. Test Clean

```bash
./build.sh Clean
```

**Attendu :** 
```
🧹 Cleaning builds...
✅ Cleaned successfully
```

---

### 3. Test Compile

```bash
./build.sh Compile
```

**Attendu :** 
```
Restoring packages...
Building MReveil...
✅ Build successful
```

---

### 4. Test PublishWindows (sur Windows)

```bash
.\build.cmd PublishWindows --configuration Release
```

**Attendu :** 
- Dossier `artifacts/windows/` créé
- Fichier `MReveil.exe` présent

---

### 5. Test CreateDMG (sur macOS uniquement)

```bash
./build.sh CreateDMG --configuration Release
```

**Attendu :**
- Fichier `artifacts/MReveil-1.0.0.dmg` créé
- Taille du DMG affichée
- Message "✅ DMG created successfully"

---

### 6. Test BuildAll

```bash
./build.sh BuildAll --configuration Release
```

**Attendu :**
- `artifacts/macos/` (sur macOS)
- `artifacts/windows/` (sur Windows)
- `artifacts/android/`
- Message "🎉 All platforms built successfully!"

---

## ✅ Checklist de validation

- [ ] `./build.sh --help` affiche les targets
- [ ] `Clean` nettoie les artifacts
- [ ] `Compile` compile sans erreur
- [ ] `PublishWindows` crée un .exe (Windows)
- [ ] `CreateDMG` crée un .dmg (macOS)
- [ ] `PublishAndroid` crée un .apk
- [ ] Les artifacts sont dans `artifacts/`
- [ ] Les logs sont colorés et clairs

---

## 🐛 Si ça ne fonctionne pas

### Erreur : "build.sh: command not found"

```bash
chmod +x build.sh
./build.sh
```

### Erreur : "Project not found"

Assurez-vous d'être dans le répertoire racine :
```bash
cd /path/to/MReveil
ls -la build/Build.cs  # Doit exister
```

### Erreur compilation Build.cs

```bash
# Restaurer les packages
dotnet restore build/build.csproj

# Recompiler
dotnet build build/build.csproj
```

### Erreur : "CreateDMG failed: hdiutil not found"

Vous n'êtes pas sur macOS. Ce target nécessite macOS.

---

## 📊 Résultats attendus

### Après `BuildAll --configuration Release`

```
artifacts/
├── macos/
│   └── MReveil.app (si sur macOS)
├── windows/
│   └── MReveil.exe
├── android/
│   └── com.monbsoft.mreveil.apk
└── MReveil-1.0.0.dmg (si CreateDMG exécuté)
```

### Tailles approximatives

- **DMG** : ~50-100 MB
- **Windows .exe** : ~80-150 MB
- **Android .apk** : ~30-60 MB

---

**Si tous les tests passent → Le build system est opérationnel !** ✅
