using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class FillArray2D
    {

        public FillArray2D()
        {

        }

        public ArrayObject[] RandomRectangles(Utils.IntVector2 density, Utils.IntVector2 minSize, Utils.IntVector2 maxSize)
        {
            ArrayObject[] objects = new ArrayObject[density.x * density.y];
            int[,] array = new int[density.y, density.x];
            Utils.IntVector2 index = new Utils.IntVector2(0, 0);
            int i = 0;
            bool filling = true;

            // LOOP
            while (filling)
            {
                // Get new end values
                int endX = Random.Range((index.x + minSize.x - 1), (index.x + maxSize.x - 1));
                int endY = Random.Range((index.y + minSize.y - 1), (index.y + maxSize.y - 1));

                // check end X
                if (endX >= density.x)
                {
                    endX = density.x - 1;
                }

                // check end Y
                if (endY >= density.y)
                {
                    endY = density.y - 1;
                }

                // create new ArrayObject
                objects[i] = new ArrayObject(index.x, index.y, endX, endY);

                // fill the array
                for (int y = index.y; y < endY + 1; y++)
                {
                    for (int x = index.x; x < endX + 1; x++)
                    {
                        array[y, x] = i + 1;
                    }
                }

                // increase i
                i++;

                // increase index.x
                index.x = endX + 1;

                // find new empty cell for index.x
                bool emptyCell = false;
                while (!emptyCell && filling)
                {
                    emptyCell = true;

                    // check index.x
                    if (index.x >= density.x)
                    {
                        index.x = 0;
                        index.y++;
                    }

                    // check if index.y is over or equal to density.y
                    if (index.y >= density.y)
                    {
                        filling = false;
                    }

                    // check index.x again
                    if (filling)
                    {
                        if (array[index.y, index.x] != 0)
                        {
                            index.x = objects[array[index.y, index.x] - 1].endX + 1;
                            emptyCell = false;
                        }
                    }
                }
            }
            // LOOP END

            ArrayObject[] temp = new ArrayObject[i];
            for (int a = 0; a < i; a++)
            {
                temp[a] = CopyArrayObject(objects[a]);
            }
            return temp;
        }

        public ArrayObject[] RandomRectangles2(Utils.IntVector2 density, Utils.IntVector2 minSize, Utils.IntVector2 maxSize)
        {
            ArrayObject[] objects = new ArrayObject[density.x * density.y];
            int filled = 0;
            int fill = 1;
            int[,] array = new int[density.y, density.x];

            Utils.IntVector2 start = new Utils.IntVector2(0, 0);
            Utils.IntVector2 end = new Utils.IntVector2(-1, 0);
            bool filling = true;


            // LOOP
            int frames = 0;
            while (filling)
            {
                // find new starting point and ending point
                start = FindStartPoint(array, start, end, density);
                end = FindEndPoint(start, density, minSize, maxSize);

                // FILL AREA
                for (int y = start.y; y <= end.y; y++)
                {
                    for (int x = start.x; x <= end.x; x++)
                    {
                        // if cell is empty, fill it
                        if (ValidCellIn2DArray(array, y, x, 0, true))
                        {
                            array[y, x] = fill;
                            filled++;
                        }
                        else
                        {
                            // if not, change end
                            x--;
                            end.x = x;
                        }
                    }
                }

                // add object
                objects[fill - 1] = new ArrayObject(start.x, start.y, end.x, end.y);
                fill++;

                // end loop when end is reached
                if (filled == density.x * density.y)
                {
                    filling = false;
                }


                // Backup plan
                frames++;
                if(frames > 1000)
                {
                    filling = false;
                }
            }

            // copy objects to a smaller list
            ArrayObject[] temp = new ArrayObject[fill -1];
            for (int a = 0; a < fill -1; a++)
            {
                temp[a] = CopyArrayObject(objects[a]);
            }
            return temp;
        }

        private Utils.IntVector2 FindStartPoint(int[,] array, Utils.IntVector2 start, Utils.IntVector2 end, Utils.IntVector2 density)
        {
            Utils.IntVector2 vector = new Utils.IntVector2
            {
                x = end.x +1,
                y = start.y
            };

            bool emptyCell = false;
            while(!emptyCell)
            {
                if (vector.x >= density.x)
                {
                    vector.x = 0;
                    vector.y++;
                }

                if(ValidCellIn2DArray(array, vector.y, vector.x, 0, true))
                {
                    emptyCell = true;
                } else
                {
                    vector.x++;
                }
            }

            return vector;
        }

        private Utils.IntVector2 FindEndPoint(Utils.IntVector2 start, Utils.IntVector2 density, Utils.IntVector2 minSize, Utils.IntVector2 maxSize)
        {
            Utils.IntVector2 vector = new Utils.IntVector2
            {
                x = Random.Range((start.x + minSize.x - 1), (start.x + maxSize.x - 1) +1),
                y = Random.Range((start.y + minSize.y - 1), (start.y + maxSize.y - 1) +1)
            };

            if (vector.x >= density.x)
            {
                vector.x = density.x - 1;
            }

            if (vector.y >= density.y)
            {
                vector.y = density.y - 1;
            }

            return vector;
        }

        private bool ValidCellIn2DArray(int[,] array, int y, int x, int value, bool same)
        {
            return array[y, x] == value && same;
        }

        private ArrayObject CopyArrayObject(ArrayObject original)
        {
            return new ArrayObject(original.startX, original.startY, original.endX, original.endY);
        }

        public struct ArrayObject
        {
            public ArrayObject(int x1, int y1, int x2, int y2)
            {
                startX = x1;
                startY = y1;
                endX = x2;
                endY = y2;
            }

            public int startX, startY, endX, endY;
        }

    }
}