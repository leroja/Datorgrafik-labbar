﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Source.Components
{
    public class HeightmapComponentTexture : IComponent
    {
        public BasicEffect Effect { get; set; }
        public int[] Indices { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }

        public HeightmapComponentTexture()
        {

        }
    }
}