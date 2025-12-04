using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace BattleShip
{
    internal class Grille
    {
        // CONSTS
        const int SIZE_MAX = 10;

        // Boat initialization
        private readonly int[] boatSize = { 5, 4, 3, 3, 2 };
        int[,] grid = new int[SIZE_MAX, SIZE_MAX];

        // Generation Random Number
        Random rdmPosition = new Random();

        public void GenRandomBoat()
        {
            // Creating random boat generation
            int row = rdmPosition.Next(0, SIZE_MAX);
            int col = rdmPosition.Next(0, SIZE_MAX);

            int i = 0;

            foreach (var boat in boatSize)
            {
                int id = boatSize[boat];
            }
        }

        public void PlaceBoat(int size, int id)
        {

        }

        public void PlacementPossible(int size, int row, int col, int direction)
        {

        }

        public enum ShotResult
        {
            Success,
            Failed,
            Down,
        }

        public void Shoot()
        {
            //return ShootResult;
        }
    }
}