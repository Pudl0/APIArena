# **APIArena**

**APIArena** is a multiplayer game API that supports both Player vs Player (PvP) and Player vs Environment (PvE) game modes. Players can move, mine resources, and store them on a map. The game concludes when certain conditions are met, such as reaching round 50 or depleting all the available gold.

## **Table of Contents**
- [Installation](#installation)
  - [Standard Installation](#standard-installation)
  - [Installation with Docker](#installation-with-docker)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
  - [New Game](#new-game)
  - [Play Game](#play-game)
- [Game Logic](#game-logic)
  - [End Game Conditions](#end-game-conditions)
  - [Player Actions](#player-actions)
  - [Bot Logic](#bot-logic)
- [Contributing](#contributing)
- [License](#license)

## **Installation**

### **Standard Installation**

1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/APIArena.git
   ```

2. **Navigate to the project directory:**
   ```sh
   cd APIArena
   ```

3. **Open the solution in Visual Studio:**
   ```sh
   APIArena.sln
   ```

4. **Restore the NuGet packages:**
   ```sh
   dotnet restore
   ```

5. **Build the project:**
   ```sh
   dotnet build
   ```

### **Installation with Docker**

If you prefer to run the application using Docker, follow these steps:

1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/APIArena.git
   ```

2. **Navigate to the project directory:**
   ```sh
   cd APIArena
   ```

3. **Build the Docker image:**
   ```sh
   docker build -t apiarena:latest .
   ```

4. **Run the Docker container:**
   ```sh
   docker run -d -p 5001:5001 --name apiarena apiarena:latest
   ```

5. **Access the API:**
   The API will be available at `http://localhost:5001`.

## **Usage**

1. **Run the application:**
   ```sh
   dotnet run
   ```

2. **Access the API:**
   The API will be available at `https://localhost:5001`.

## **API Endpoints**

### **New Game**

- **Endpoint:** `POST /api/v1/GameController/newgame`
- **Description:** Starts a new game.
- **Headers:**
  - `X-API-Key: {apiKey}`
- **Response:**
  - `200 OK`: Game successfully created.
  - `400 Bad Request`: Invalid request.
  - `401 Unauthorized`: Invalid API key.

### **Play Game**

- **Endpoint:** `POST /api/v1/GameController/play`
- **Description:** Executes a turn in the game.
- **Headers:**
  - `X-API-Key: {apiKey}`
- **Parameters:**
  - `gameId: {Guid}`
  - `action: {string}` (e.g., `MoveTop`, `MoveBottom`, `MoveLeft`, `MoveRight`, `MineRessource`, `StoreRessource`)
- **Response:**
  - `200 OK`: Move successfully executed.
  - `400 Bad Request`: Invalid request.
  - `401 Unauthorized`: Invalid API key.
  - `404 Not Found`: Game or player not found.

## **Game Logic**

### **End Game Conditions**

The game ends when any of the following conditions are met:

- Round 50 has passed.
- All gold has been mined from the map.

### **Player Actions**

Players can perform the following actions:

- `MoveTop`: Move the player up.
- `MoveBottom`: Move the player down.
- `MoveLeft`: Move the player left.
- `MoveRight`: Move the player right.
- `MineRessource`: Mine resources from the current tile.
- `StoreRessource`: Store mined resources at the base.

### **Bot Logic**

The bot player will:

- Mine resources if on a gold tile.
- Move towards the nearest gold tile if not carrying gold.
- Move towards the base to store resources if carrying gold.

## **Contributing**

Contributions are welcome! Please [open an issue](https://github.com/yourusername/APIArena/issues) or submit a pull request for any changes.

## **License**

This project is licensed under the [MIT License](LICENSE). See the LICENSE file for details.