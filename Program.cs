using System;

using System.Collections.Generic;

  //By Vampy

namespace SpaceInvaders

{

    class Program
    {

        static int playerLives = 3; // Contador de vidas del jugador

        static int enemiesRemaining = 0; // Contador de enemigos vivos

  

        static void Main()

        {

            // Tamaño de la habitación

            int width = 20;

            int height = 20;

  

            // Crear la habitación

            char[,] room = new char[width, height];

  

            // Rellenar la habitación con paredes, goblins y enemigos

            Random random = new Random();

            for (int y = 0; y < height; y++)

            {

                for (int x = 0; x < width; x++)

                {

                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)

                    {

                        room[x, y] = '#'; // Paredes exteriores

                    }

                    else if (random.Next(10) < 3 && enemiesRemaining < 20) // Probabilidad de 30% de que haya un goblin o un enemigo en cada celda

                    {

                        if (random.Next(2) == 0)

                        {

                            room[x, y] = 'G'; // Goblin

                        }

                        else

                        {

                            room[x, y] = '@'; // Enemigo

                        }

                        enemiesRemaining++;

                    }

                    else

                    {

                        room[x, y] = ' ';

                    }

                }

            }

  

            // Posición inicial del jugador más cerca del centro de la habitación

            int playerX = width / 2;

            int playerY = height - 2;

            if (width % 2 == 0)

            {

                playerX--; // Si el ancho es par, mover una posición a la izquierda

            }

            room[playerX, playerY] = '^'; // Flecha hacia arriba para representar al jugador

  

            // Imprimir la habitación

            PrintRoom(room);

  

            // Lista para almacenar la posición de los disparos

            var shots = new List<(int x, int y)>();

  

            // Esperar entrada del jugador

            while (playerLives > 0 && enemiesRemaining > 0)

            {

                // Mover los disparos

                MoveShots(room, shots);

  

                // Leer la entrada del jugador

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

  

                // Mover al jugador, disparar o salir del juego según la tecla presionada

                switch (keyInfo.Key)

                {

                    case ConsoleKey.LeftArrow:

                        MovePlayer(room, ref playerX, ref playerY, -1, 0);

                        break;

                    case ConsoleKey.RightArrow:

                        MovePlayer(room, ref playerX, ref playerY, 1, 0);

                        break;

                    case ConsoleKey.UpArrow:

                        MovePlayer(room, ref playerX, ref playerY, 0, -1);

                        break;

                    case ConsoleKey.Spacebar:

                        Shoot(room, playerX, playerY, shots);

                        break;

                    case ConsoleKey.Escape:

                        Environment.Exit(0); // Salir del juego

                        break;

                    default:

                        break;

                }

  

                // Limpiar la consola y volver a imprimir la habitación

                Console.Clear();

                PrintRoom(room);

  

                // Eliminar disparos que hayan alcanzado el borde superior

                RemoveOutOfBoundsShots(shots, height);

            }

  

            if (enemiesRemaining == 0)

            {

                Console.WriteLine("¡Has Matado a todos los enemigos! ¡You Win!");

            }

            else

            {

                Console.WriteLine("¡Perdiste Bro! Perdiste todas las vidas.");

            }

        }

  

        // Función para imprimir la habitación en la consola

        static void PrintRoom(char[,] room)

        {

            int width = room.GetLength(0);

            int height = room.GetLength(1);

  

            for (int y = 0; y < height; y++)

            {

                for (int x = 0; x < width; x++)

                {

                    Console.Write(room[x, y] + " ");

                }

                Console.WriteLine();

            }

  

            Console.WriteLine($"Vidas del jugador: {playerLives}");

        }

  

        // Función para mover al jugador en la habitación

        static void MovePlayer(char[,] room, ref int playerX, ref int playerY, int deltaX, int deltaY)

        {

            int newX = playerX + deltaX;

            int newY = playerY + deltaY;

  

            // Verificar si la nueva posición es válida (no es una pared)

            if (room[newX, newY] != '#')

            {

                // Verificar si hay un enemigo en la nueva posición

                if (room[newX, newY] == 'G' || room[newX, newY] == '@')

                {

                    // El jugador colisionó con un goblin o un enemigo, pierde una vida

                    playerLives--;

  

                    // Reiniciar la posición del jugador

                    playerX = room.GetLength(0) / 2;

                    playerY = room.GetLength(1) - 2;

                }

                else

                {

                    // Mover al jugador

                    room[playerX, playerY] = ' ';

                    playerX = newX;

                    playerY = newY;

                    room[playerX, playerY] = '^';

                }

            }

        }

  

        // Función para que el jugador dispare

        static void Shoot(char[,] room, int playerX, int playerY, List<(int x, int y)> shots)

        {

            // Agregar un disparo a la posición del jugador

            shots.Add((playerX, playerY - 1));

        }

  

        // Función para mover los disparos

        static void MoveShots(char[,] room, List<(int x, int y)> shots)

        {

            // Iterar sobre los disparos y moverlos hacia arriba

            for (int i = 0; i < shots.Count; i++)

            {

                int x = shots[i].x;

                int y = shots[i].y;

  

                // Verificar si hay un enemigo en la trayectoria del disparo

                if (room[x, y - 1] == 'G' || room[x, y - 1] == '@')

                {

                    // Eliminar al goblin o al enemigo

                    room[x, y - 1] = ' ';

                    enemiesRemaining--; // Decrementar el contador de enemigos vivos

                    shots.RemoveAt(i);

                    i--; // Ajustar el índice después de eliminar un disparo

                }

                else if (y - 1 >= 0 && room[x, y - 1] != '#') // Verificar si el disparo está dentro de los límites de la habitación y no choca contra una pared

                {

                    // Mover el disparo hacia arriba

                    room[x, y] = ' ';

                    shots[i] = (x, y - 1);

                    room[x, y - 1] = '-';

                }

            }

        }

  

        // Función para eliminar disparos que hayan alcanzado el borde superior

        static void RemoveOutOfBoundsShots(List<(int x, int y)> shots, int height)

        {

            for (int i = 0; i < shots.Count; i++)

            {

                if (shots[i].y <= 0)

                {

                    shots.RemoveAt(i);

                    i--; // Ajustar el índice después de eliminar un disparo

                }

            }

        }

    }

}