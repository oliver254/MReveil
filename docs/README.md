# MReveil - Documentation Technique

## 📋 Table des matières

1. [Vue d'ensemble](#vue-densemble)
2. [Spécifications](./specifications.md) ⭐ **NOUVEAU**
3. [Architecture](./architecture.md)
4. [Diagrammes UML](./uml-diagrams.md)
5. [Modèle de données](./data-model.md)
6. [Gestion de projet](./project-management.md)
7. [Guide de développement](./development-guide.md)
8. [Association Tâche-Pomodoro](./task-pomodoro-association.md) ⭐ **NOUVEAU**

## Vue d'ensemble

**MReveil** est une application de gestion du temps basée sur la technique Pomodoro, développée avec .NET MAUI pour une expérience multiplateforme (iOS, Android, macOS, Windows).

### 🎯 Objectifs

- Améliorer la productivité avec la technique Pomodoro
- Suivre les sessions de travail et de pause
- **Gérer les tâches quotidiennes** ⭐ **NOUVEAU**
- Visualiser les statistiques de productivité
- Tenir un journal quotidien des activités

### 🏗️ Technologies

- **.NET 10** - Framework principal
- **.NET MAUI** - Framework multiplateforme
- **SQLite** - Base de données locale
- **CommunityToolkit.Mvvm** - Architecture MVVM
- **CommunityToolkit.Maui** - Composants UI additionnels

### 📦 Packages NuGet

| Package | Version | Usage |
|---------|---------|-------|
| CommunityToolkit.Maui | 13.0.0 | Composants UI et behaviors |
| CommunityToolkit.Maui.MediaElement | 7.0.0 | Lecteur média pour les alarmes |
| CommunityToolkit.Mvvm | 8.4.0 | Architecture MVVM |
| sqlite-net-pcl | 1.9.172 | ORM SQLite |
| SQLitePCLRaw.bundle_green | 2.1.10 | Runtime SQLite natif |

### 🎨 Fonctionnalités principales

1. **Timer Pomodoro**
   - Pomodoro (25 min par défaut)
   - Pause courte (5 min par défaut)
   - Pause longue (15 min par défaut)

2. **Affichage circulaire**
   - Horloge en temps réel
   - Compte à rebours visuel
   - Animation fluide

3. **Gestion des tâches** ⭐ **NOUVEAU**
   - Créer des tâches quotidiennes
   - Marquer les tâches comme complétées
   - Associer des tâches aux sessions Pomodoro
   - Suivi du nombre de Pomodoros par tâche
   - Suppression de tâches

4. **Suivi des sessions**
   - Enregistrement automatique
   - Statistiques journalières
   - Historique complet
   - Association aux tâches

5. **Journal**
   - Statistiques quotidiennes (Pomodoros, Focus Time, Tasks)
   - **Gestion des tâches du jour** ✅
   - **Affichage des tâches complétées** ⭐

6. **Statistiques**
   - Graphiques de productivité
   - Séries de jours consécutifs
   - Métriques mensuelles
   - **Taux de complétion des tâches** ⭐ **NOUVEAU**

## 🚀 Démarrage rapide

### Prérequis

- Visual Studio 2022 (version 17.12 ou supérieure)
- .NET 10 SDK
- Charges de travail MAUI installées

### Installation

```bash
git clone https://github.com/oliver254/MReveil.git
cd MReveil
dotnet restore
dotnet build
```

### Exécution

```bash
# Windows
dotnet run --framework net10.0-windows10.0.19041.0

# Android
dotnet run --framework net10.0-android

# iOS
dotnet run --framework net10.0-ios

# macOS
dotnet run --framework net10.0-maccatalyst
```

## 📱 Plateformes supportées

| Plateforme | Version minimale | Statut |
|------------|------------------|--------|
| Windows | 10.0.19041.0 | ✅ Supporté |
| Android | API 24 (Android 7.0) | ✅ Supporté |
| iOS | 15.0 | ✅ Supporté |
| macOS (Catalyst) | 15.0 | ✅ Supporté |

## 🏛️ Architecture

L'application suit le pattern **MVVM (Model-View-ViewModel)** avec une séparation claire des responsabilités :

```
src/MReveil/
├── Models/           # Modèles de données et états
│   ├── TodoTask.cs   # ⭐ NOUVEAU
├── ViewModels/       # Logique de présentation
├── Views/            # Interfaces utilisateur (XAML)
├── Services/         # Services métier
│   ├── TaskService.cs   # ⭐ NOUVEAU
├── Controls/         # Contrôles personnalisés
├── Drawables/        # Rendus graphiques personnalisés
├── Messaging/        # Messages pour la communication inter-composants
├── Converters/       # Value Converters  # ⭐ NOUVEAU
└── Resources/        # Ressources (images, styles, polices)
```

## 📊 Nouveautés - Version 2.0

### Gestion des tâches

L'application permet maintenant de gérer une liste de tâches quotidiennes avec :

- **Création rapide** : Ajoutez une tâche en quelques secondes
- **Suivi Pomodoro** : Associez une tâche à une session Pomodoro
- **Compteur de Pomodoros** : Voyez combien de sessions vous avez consacrées à chaque tâche
- **Complétion** : Marquez vos tâches comme terminées
- **Statistiques** : Le nombre de tâches complétées est maintenant affiché dans le journal

### Modèle de données étendu

```
Tasks (Nouvelle table)
├── Titre
├── État (complétée/non complétée)
├── Date
├── Compteur de Pomodoros
├── Estimation
└── Priorité

PomodoroSessions (Mise à jour)
└── TaskId (lien vers la tâche)

JournalEntries (Mise à jour)
└── TasksCompleted (nombre de tâches terminées)
```

## 📄 Documentation

La documentation complète est organisée en plusieurs fichiers spécialisés :

1. **[Spécifications](./specifications.md)** ⭐ **NOUVEAU**
   - Analyse des besoins fonctionnels et non-fonctionnels
   - Modélisation des exigences avec diagrammes UML
   - Cas d'utilisation détaillés
   - Scénarios d'interactions
   - Règles métier
   - Contraintes techniques
   - Glossaire

2. **[Architecture](./architecture.md)**
   - Vue d'ensemble de l'architecture en couches
   - Composants principaux et leurs responsabilités
   - Flux de données et communication
   - Patterns utilisés (MVVM, DI, Messaging)

3. **[Diagrammes UML](./uml-diagrams.md)**
   - Diagrammes de classes complets
   - Diagrammes de séquence pour les flux principaux
   - Diagrammes d'états
   - Diagrammes d'activités
   - Diagrammes de composants et de déploiement

4. **[Modèle de données](./data-model.md)**
   - Schéma de base de données détaillé
   - Tables et relations
   - Index et optimisations
   - Exemples de requêtes
   - Migrations

5. **[Gestion de projet](./project-management.md)**
   - Roadmap du projet
   - Backlog produit
   - Sprints et planning
   - Métriques et indicateurs
   - Processus de développement

6. **[Guide de développement](./development-guide.md)**
   - Configuration de l'environnement
   - Conventions de code
   - Bonnes pratiques
   - Tests
   - Débogage
   - Déploiement

7. **[Association Tâche-Pomodoro](./task-pomodoro-association.md)** ⭐ **NOUVEAU**
   - Lien entre les tâches et les sessions Pomodoro
   - Amélioration du suivi des tâches
   - Gestion des interruptions
   - Historique des associations

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](../LICENSE) pour plus de détails.

## 👥 Contributeurs

- Olivier254 - Développeur principal

## 📞 Contact

Pour toute question ou suggestion, ouvrez une issue sur [GitHub](https://github.com/oliver254/MReveil/issues).

---

**Dernière mise à jour:** 16 Janvier 2024 - Version 2.0  
**Statut:** En développement actif  
**Branche:** Add-journal
