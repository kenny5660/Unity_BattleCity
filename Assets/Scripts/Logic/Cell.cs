using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.Logic
{
    internal enum CellSpace
    {
        Empty,
        Bedrock,
        Flag,
        Grass
    }

    internal class Cell
    {
        public CellSpace Space { get; private set; }
        public Tank Occupant;
        public FieldObject fieldObject;

        public Cell(CellSpace space, FieldObject fieldObject = null)
        {
            Space = space;
            this.fieldObject = fieldObject;
        }
        public bool TryDestroyCell()
        {
            if (Space == CellSpace.Grass || Space == CellSpace.Flag)
            {
                Space = CellSpace.Empty;
                return true;
            }
            return false;
        }
        public void Occupy(Tank occupant)
        {
            Occupant = occupant;
        }
    }
}
