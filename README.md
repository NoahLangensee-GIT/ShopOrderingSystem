
## Voraussetzungen

Bevor du die Anwendung starten kannst, benötigst du folgende Software:

### 1. .NET 8 SDK installieren

**Option A: Manuelle Installation**
- Besuche die [.NET Download-Seite](https://dotnet.microsoft.com/download/dotnet/8.0)
- Lade das **.NET 8 SDK** herunter (nicht nur die Runtime)
- Installiere das SDK nach den Anweisungen für dein Betriebssystem
- Überprüfe die Installation mit:
  ```powershell
  dotnet --version
  ```

**Option B: Installation über JetBrains Rider (empfohlen)**
- Öffne das Projekt in JetBrains Rider
- Rider wird automatisch erkennen, dass .NET 8 SDK fehlt
- Rider bietet an, das SDK automatisch herunterzuladen und einzurichten
- Folge den Anweisungen im Dialog

### 2. Docker installieren

- Wenn nicht bereits installiert, lade [Docker](https://www.docker.com/products/docker-desktop) herunter
- Installiere Docker Desktop für dein Betriebssystem
- Starte Docker Desktop

## Setup und Start

### Schritt 1: MySQL Docker Container starten

Starte den MySQL-Container mit den folgenden Einstellungen:

```powershell
docker run --name mysql-todo -e MYSQL_ROOT_PASSWORD=Test -e MYSQL_DATABASE=todo -p 3307:3306 -d mysql:8.0
```

**Oder starten Sie einen bestehenden Container neu:**
```powershell
docker start mysql-todo
```

### Schritt 2: Anwendung starten

#### Option A: Über JetBrains Rider (Einfach)
1. Öffne das Projekt in **JetBrains Rider**
2. Warte bis Rider alle NuGet-Pakete automatisch heruntergeladen und wiederhergestellt hat
3. Klicke auf den **grünen Play-Button** ▶ oben rechts
4. Wähle `TODOApp` aus der Run-Konfiguration
5. Die Anwendung startet automatisch

#### Option B: Über PowerShell/Terminal
1. Öffne ein PowerShell/Terminal im Projekt-Verzeichnis
2. Stelle sicher, dass alle NuGet-Pakete heruntergeladen sind:
   ```powershell
   dotnet restore "TODO App.sln"
   ```
3. Baue die Lösung:
   ```powershell
   dotnet build "TODO App.sln"
   ```
4. Starte die Anwendung:
   ```powershell
   dotnet run --project TODOApp/TODOApp.csproj
   ```

## Konfiguration

Die Datenbankverbindung ist in `TODOApp.Business/Business/SessionFactory.cs` konfiguriert:

- **Server:** localhost
- **Port:** 3307
- **Database:** todo
- **User:** root
- **Password:** Test

Wenn du andere Einstellungen benötigst, bearbeite die Connection String in dieser Datei.

## NuGet Pakete

Das Projekt verwendet folgende NuGet-Pakete:

- **FluentNHibernate** - ORM für Datenbankzugriff
- **NHibernate** - Object-Relational Mapping Framework
- **MySql.Data** - MySQL Database Connector
- **Autofac** - Dependency Injection Container
- **BouncyCastle.Cryptography** - Kryptographie-Bibliothek

Die Pakete werden automatisch heruntergeladen und installiert, wenn du das Projekt öffnest oder `dotnet restore` ausführst.

## Troubleshooting

### Docker-Container Status prüfen
```powershell
docker ps
```

### Container-Logs anschauen
```powershell
docker logs mysql-todo
```

### Container stoppen
```powershell
docker stop mysql-todo
```

### Container entfernen (um neu zu starten)
```powershell
docker rm mysql-todo
```" 
