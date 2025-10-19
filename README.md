# 🏦 EKBank - Application Bancaire Full-Stack

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![React](https://img.shields.io/badge/React-19.2.0-61DAFB?style=for-the-badge&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-4.9.5-3178C6?style=for-the-badge&logo=typescript)
![Material-UI](https://img.shields.io/badge/Material--UI-6.5.0-007FFF?style=for-the-badge&logo=mui)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)

## 📋 Description du Projet

EKBank est une application bancaire développée avec une architecture full-stack robuste. Elle permet la gestion des comptes bancaires, des transactions et de l'authentification des clients avec une interface utilisateur sécurisée.

## 🚀 Technologies et Compétences Mises en Œuvre

### 🔧 Backend (.NET 8.0)

#### **Frameworks & Technologies**
- **ASP.NET Core 8.0 en C#** 
- **Entity Framework Core 8.0** - ORM pour la gestion de base de données
- **SQL Server** - Base de données relationnelle
- **Swagger/OpenAPI** - Documentation automatique de l'API

#### **Sécurité & Authentification**
- **JWT (JSON Web Tokens)** - Authentification stateless sécurisée
- **BCrypt.Net** - Hachage sécurisé des mots de passe
- **Microsoft.AspNetCore.Authentication.JwtBearer** - Middleware d'authentification JWT

#### **Architecture & Patterns**
- **Architecture en couches** (Controllers, Services, Data, Models)
- **Repository Pattern** avec Entity Framework
- **Dependency Injection** natif d'ASP.NET Core
- **DTOs (Data Transfer Objects)** pour la sérialisation
- **Code First Migrations** pour la gestion de schéma de base de données

#### **Fonctionnalités Avancées**
- **Gestion des CORS** pour les appels cross-origin
- **Logging** intégré avec ILogger
- **Configuration flexible** (appsettings.json, variables d'environnement)
- **Gestion des erreurs** centralisée
- **Validation des modèles** avec Data Annotations

### 🗄️ Base de Données (Microsoft SQL Server)

#### **Configuration & Architecture**
- **Microsoft SQL Server** - Système de gestion de base de données relationnelle
- **Entity Framework Core 8.0** - ORM avec support SQL Server
- **Code First Approach** - Modèles définis en C# puis migrés vers SQL Server
- **Connection String** configurée dans `appsettings.json`
- **Migrations automatiques** pour la gestion du schéma

#### **Structure de la Base de Données**
```sql
-- Tables principales
├── Clients          # Informations des utilisateurs
│   ├── Id (PK)      # Clé primaire auto-incrémentée
│   ├── Nom          # Nom du client
│   ├── Email        # Email unique
│   └── PasswordHash # Mot de passe haché (BCrypt)
│
├── Comptes          # Comptes bancaires
│   ├── Id (PK)      # Clé primaire auto-incrémentée
│   ├── NumeroCompte # Numéro unique du compte
│   ├── Solde        # Solde actuel (decimal)
│   ├── TypeCompte   # Type de compte (Courant/Épargne)
│   └── ClientId (FK)# Référence vers Clients
│
└── Transactions     # Historique des transactions
    ├── Id (PK)      # Clé primaire auto-incrémentée
    ├── Montant      # Montant de la transaction
    ├── Type         # Type (Débit/Crédit)
    ├── Description  # Description de la transaction
    ├── DateTransaction # Date et heure
    └── CompteId (FK)# Référence vers Comptes
```

#### **Fonctionnalités SQL Server Utilisées**
- **Identity Columns** - Auto-incrémentation des clés primaires
- **Foreign Keys** - Relations entre tables avec contraintes d'intégrité
- **Indexes** - Optimisation des requêtes sur Email et NumeroCompte
- **Decimal Precision** - Gestion précise des montants financiers
- **DateTime2** - Stockage précis des dates de transaction
- **Trusted Connection** - Authentification Windows intégrée

### 🎨 Frontend (React + TypeScript)

#### **Frameworks & Bibliothèques**
- **React 19.2.0** - Bibliothèque UI moderne avec les dernières fonctionnalités
- **TypeScript 4.9.5** - Typage statique pour un code plus robuste
- **Material-UI (MUI) 6.5.0** - Composants UI élégants et accessibles
- **React Router DOM 7.9.4** - Navigation côté client

#### **Gestion d'État & Data Fetching**
- **Redux Toolkit 2.9.1** - Gestion d'état prévisible et moderne
- **React-Redux 9.2.0** - Intégration React-Redux optimisée
- **Axios 1.12.2** - Client HTTP avec intercepteurs JWT

#### **Formulaires & Validation**
- **Formik 2.4.6** - Gestion avancée des formulaires
- **Yup 1.7.1** - Validation de schémas robuste

#### **Visualisation de Données**
- **Recharts 3.3.0** - Graphiques interactifs et responsives

#### **Qualité & Tests**
- **Testing Library** - Tests unitaires et d'intégration
- **Jest** - Framework de tests
- **ESLint** - Analyse statique du code
- **TypeScript strict mode** - Vérifications de types strictes

### 🏗️ Architecture Globale

#### **Modèle de Données**
- **Client** - Gestion des utilisateurs avec authentification
- **Compte** - Comptes bancaires avec soldes
- **Transaction** - Historique des opérations bancaires

#### **API RESTful**
- **AuthController** - Authentification et gestion des sessions
- **ClientsController** - Gestion des profils clients
- **ComptesController** - Opérations sur les comptes
- **TransactionsController** - Gestion des transactions

#### **Sécurité Implémentée**
- 🔐 **Authentification JWT** avec expiration configurable
- 🛡️ **Hachage BCrypt** pour les mots de passe
- 🔒 **Routes protégées** côté frontend et backend
- 🚫 **Validation des entrées** sur tous les endpoints
- 🔑 **Gestion sécurisée des tokens** avec localStorage

## 🌟 Fonctionnalités Principales

### 👤 Gestion des Utilisateurs
- ✅ Inscription et connexion sécurisées
- ✅ Profils clients personnalisés
- ✅ Authentification JWT persistante

### 💰 Gestion Bancaire
- ✅ Visualisation des comptes multiples
- ✅ Historique des transactions en temps réel
- ✅ Tableaux de bord interactifs avec graphiques
- ✅ Calculs de soldes automatiques

### 📊 Interface Utilisateur
- ✅ Design Material Design moderne
- ✅ Interface responsive et accessible
- ✅ Graphiques interactifs (Recharts)
- ✅ Navigation fluide (React Router)

## 📁 Structure du Projet

```
EKBank/
├── backend/
│   └── Banque.API/
│       ├── Controllers/          # Contrôleurs API REST
│       ├── Models/               # Modèles de données
│       ├── DTOs/                 # Data Transfer Objects
│       ├── Data/                 # Contexte Entity Framework
│       ├── Migrations/           # Migrations de base de données
│       └── Program.cs            # Configuration de l'application
└── frontend/
    └── src/
        ├── components/           # Composants React réutilisables
        ├── pages/               # Pages principales (Login, Dashboard)
        ├── store/               # Configuration Redux
        ├── services/            # Services API (Axios)
        ├── types/               # Types TypeScript
        └── App.tsx              # Composant racine avec routing
```

---

## 🧪 Guide de Test de l'Application

### 📋 Prérequis

Avant de commencer les tests, assurez-vous d'avoir installé :

- **Node.js** (version 16 ou supérieure)
- **.NET 8.0 SDK**
- **Microsoft SQL Server** (une des options suivantes) :
  - **SQL Server LocalDB** (recommandé pour le développement)
  - **SQL Server Express** (gratuit)
  - **SQL Server Developer Edition** (gratuit pour le développement)
  - **Instance SQL Server complète**
- **Git** pour cloner le projet

#### **Installation de SQL Server LocalDB (Recommandé)**

**Windows :**
```bash
# Télécharger et installer SQL Server LocalDB
# Depuis : https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

# Vérifier l'installation
sqllocaldb info

# Créer une instance (si nécessaire)
sqllocaldb create "MSSQLLocalDB"
sqllocaldb start "MSSQLLocalDB"
```

**Configuration de la Connection String :**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BanqueDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 🔧 Installation et Configuration

#### 1. **Cloner le Projet**
```bash
git clone <url-du-repo>
cd EKBank
```

#### 2. **Configuration du Backend**

```bash
# Naviguer vers le dossier backend
cd backend/Banque.API

# Restaurer les packages NuGet
dotnet restore

# Gestion de la base de données SQL Server
dotnet ef database update

# (Optionnel) Créer une nouvelle migration si nécessaire
dotnet ef migrations add NomDeLaMigration

# Commandes utiles pour la base de données
dotnet ef database drop          # Supprimer la base de données
dotnet ef migrations list        # Lister toutes les migrations
dotnet ef migrations remove      # Supprimer la dernière migration
dotnet ef database update 0      # Revenir à l'état initial (vide)
```

#### 3. **Configuration du Frontend**

```bash
# Naviguer vers le dossier frontend
cd ../../frontend

# Installer les dépendances npm
npm install

# Vérifier que toutes les dépendances sont installées
npm audit
```

### 🚀 Démarrage de l'Application

#### **Étape 1 : Démarrer le Backend**

```bash
cd backend/Banque.API
dotnet run
```

✅ **Vérifications :**
- Le serveur démarre sur `https://localhost:5029`
- Swagger UI accessible sur `https://localhost:5029/swagger`
- Aucune erreur de connexion à la base de données

#### **Étape 2 : Démarrer le Frontend**

```bash
cd frontend
npm start
```

✅ **Vérifications :**
- L'application React démarre sur `http://localhost:3000`
- Aucune erreur de compilation TypeScript
- L'interface de connexion s'affiche correctement

### 🔑 Données de Démonstration

**Utilisateur de test pré-configuré :**

```json
{
  "email": "demo@bank.com",
  "password": "P@ssw0rd!"
}
```

> 💡 **Note :** Ces identifiants sont déjà configurés dans la base de données et permettent de tester immédiatement toutes les fonctionnalités de l'application sans avoir besoin de créer un nouveau compte.

### 🧪 Scénarios de Test

#### **Test 1 : Authentification**

1. **Accéder à l'application** : `http://localhost:3000`
2. **Vérifier la redirection** vers `/login`
3. **Tester la connexion** avec des identifiants valides
4. **Vérifier le token JWT** dans le localStorage du navigateur
5. **Tester la déconnexion** et la suppression du token

**🔑 Données de démonstration :**
```json
{
  "email": "demo@bank.com",
  "password": "P@ssw0rd!"
}
```

**Identifiants de test :**
- **Email :** `demo@bank.com`
- **Mot de passe :** `P@ssw0rd!`

#### **Test 2 : Dashboard et Données**

1. **Après connexion**, vérifier l'affichage du dashboard
2. **Contrôler les comptes** affichés avec soldes
3. **Vérifier l'historique** des transactions
4. **Tester les graphiques** Recharts (interactivité)
5. **Valider la navigation** entre les sections

#### **Test 3 : API Backend**

**Via Swagger UI** (`https://localhost:5029/swagger`) :

1. **Tester l'endpoint de login** :
   ```json
   POST /api/Auth/login
   {
     "email": "demo@bank.com",
     "password": "P@ssw0rd!"
   }
   ```

2. **Utiliser le token JWT** pour les endpoints protégés :
   - `GET /api/Clients/{id}`
   - `GET /api/Clients/{id}/comptes`
   - `GET /api/Clients/{id}/transactions`

3. **Vérifier les réponses** et codes de statut HTTP

#### **Test 4 : Sécurité**

1. **Tester l'accès sans token** aux routes protégées
2. **Vérifier l'expiration** du token JWT (après 60 minutes)
3. **Tester les validations** des formulaires
4. **Contrôler les erreurs** d'authentification

### 🔍 Tests Automatisés

#### **Tests Frontend**
```bash
cd frontend
npm test
```

#### **Tests Backend**
```bash
cd backend/Banque.API
dotnet test
```

### 📊 Métriques de Performance

#### **Outils de Monitoring**
- **React DevTools** pour l'analyse des composants
- **Redux DevTools** pour le debugging d'état
- **Network Tab** pour analyser les appels API
- **Lighthouse** pour les performances web

### 🐛 Résolution des Problèmes Courants

#### **Problème : Erreur de connexion à la base de données**
```bash
# Vérifier la chaîne de connexion dans appsettings.json
# Recréer la base de données
dotnet ef database drop
dotnet ef database update
```

#### **Problème : Erreur CORS**
```bash
# Vérifier la configuration CORS dans Program.cs
# S'assurer que l'origine frontend est autorisée
```

#### **Problème : Token JWT invalide**
```bash
# Vérifier la clé JWT dans appsettings.json
# Contrôler l'expiration du token
# Effacer le localStorage et se reconnecter
```

### ✅ Checklist de Test Complet

- [ ] Backend démarre sans erreur
- [ ] Frontend compile et démarre
- [ ] Base de données accessible
- [ ] Swagger UI fonctionnel
- [ ] Authentification opérationnelle
- [ ] Dashboard affiche les données
- [ ] Graphiques interactifs
- [ ] Navigation fluide
- [ ] Déconnexion fonctionne
- [ ] API répond correctement
- [ ] Sécurité JWT active
- [ ] Tests automatisés passent

---


**Développé par Edwin Koumba**  
*Développeur Full-Stack passionné*

