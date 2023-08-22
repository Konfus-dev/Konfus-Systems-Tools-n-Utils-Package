﻿using System.Collections.Generic;
using UnityEngine;

namespace Konfus.Systems.Grid
{
    public class Node : INode
    {
        private readonly GridBase _grid;
        private readonly Vector3Int _gridPosition;

        public Node(GridBase owningGrid, Vector3Int gridPositionOnGrid)
        {
            _grid = owningGrid;
            _gridPosition = gridPositionOnGrid;
        }

        public GridBase OwningGrid => _grid;
        
        public Vector3Int GridPosition => _gridPosition;
        public virtual Vector3 WorldPosition => _grid.WorldPosFromGridPos(_gridPosition.x, _gridPosition.y, _gridPosition.z);
        
        public virtual INode[] Neighbors
        {
            get
            {
                _neighbors ??= CalculateNeighbors();
                return _neighbors;
            }
            set => _neighbors = value;
        }
        private INode[] _neighbors;

        private INode[] CalculateNeighbors()
        {
            var neighbors = new List<INode>();
                
            Vector3Int[] potentialNeighborPositions =
            {
                GridPosition + new Vector3Int(0, 1, 0), 
                GridPosition + new Vector3Int(0, -1, 0),
                GridPosition + new Vector3Int(1, 0, 0), 
                GridPosition + new Vector3Int(-1, 0, 0),
                GridPosition + new Vector3Int(0, 0, 1), 
                GridPosition + new Vector3Int(0, 0, -1),
            };
                
            foreach (var potentialNeighborPosition in potentialNeighborPositions)
            {
                bool isPosInGridBounds = _grid.InGridBounds(
                    potentialNeighborPosition.x,
                    potentialNeighborPosition.y,
                    potentialNeighborPosition.z);
                if (!isPosInGridBounds) continue;
                    
                var neighbor = _grid.GetNode(potentialNeighborPosition.x, potentialNeighborPosition.y, potentialNeighborPosition.z);
                if (neighbor != null) neighbors.Add(neighbor);
            }
                
            return neighbors.ToArray();
        }
    }
}