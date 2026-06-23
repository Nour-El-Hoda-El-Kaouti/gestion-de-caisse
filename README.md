# 🚌 Application Web de Gestion de la Caisse (ALSA / CasaBus)

## 📖 Description du Projet
Ce projet, réalisé dans le cadre d'un **Projet de Fin d'Études (PFE)**, consiste en la conception et le développement d'une application web de gestion de la caisse destinée à la société de transport **ALSA Transport** (réseau **CasaBus** à Casablanca). 

L'application vise à remplacer la gestion manuelle et décentralisée (via fichiers Excel) par une solution informatisée, centralisée et sécurisée, permettant de suivre en temps réel les transactions financières, les feuilles de route, les conducteurs et le parc automobile.

## 🎯 Problématique Résolue
La gestion traditionnelle des caisses posait plusieurs défis majeurs :
* ❌ Erreurs humaines et incohérences comptables.
* ❌ Difficulté de conciliation des flux de trésorerie.
* ❌ Risques accrus de fraude et manque de traçabilité.
* ❌ Inefficacité opérationnelle due à la saisie manuelle.
* ❌ Complexité de gestion des divers modes de paiement.

## ✨ Fonctionnalités Principales
L'application offre une interface intuitive basée sur le modèle **MVC**, offrant les modules suivants :

* 🔐 **Authentification & Sécurité** : Connexion et inscription sécurisées pour protéger les données sensibles.
* 📄 **Gestion des Feuilles de Route (FDR)** : Visualisation détaillée, filtrage (par date, véhicule, conducteur, ligne) et recherche rapide.
* 👨‍✈️ **Gestion des Machinistes** : CRUD complet (Ajout, Modification, Suppression) et filtrage des conducteurs.
* 🚌 **Gestion des Véhicules** : Suivi du parc automobile (matricule, modèle, capacité).
* 🗺️ **Gestion des Lignes** : Consultation des itinéraires, tarifs appliqués et centres opérationnels associés.
* 💰 **Gestion des Encaissements** : 
  * Ajout et modification des transactions.
  * Règlement des manquants (écarts de caisse).
  * Justification détaillée des manquants avec motifs et commentaires pour l'audit.

## 🛠️ Stack Technique
* **Langage de programmation** : C#
* **Framework Backend** : ASP.NET Core
* **Frontend / UI** : Bootstrap (Design responsive et moderne)
* **Base de données** : SQL Server (SGBD relationnel)
* **Architecture** : MVC (Model-View-Controller)
* **IDE** : Visual Studio

## 🚀 Installation et Exécution

### Prérequis
* [.NET SDK](https://dotnet.microsoft.com/download) (Version compatible ASP.NET Core)
* [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (ou SQL Server Express / LocalDB)
* Visual Studio 2022 (recommandé)

### Étapes d'installation
1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/Nour-El-Hoda-El-Kaouti/gestion-de-caisse.git  
