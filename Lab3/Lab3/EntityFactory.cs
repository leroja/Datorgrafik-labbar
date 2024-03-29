﻿using Engine.Source.Components;
using Engine.Source.Managers;
using Lab3.GameComponents;
using Lab3.Humanoid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class EntityFactory
    {
        ContentManager Content;
        Effect shader;
        public EntityFactory(ContentManager Content)
        {
            this.Content = Content;
            shader = Content.Load<Effect>("TestShaders");
        }
        public void CreateSkyBox()
        {
            int skyboxEntity = ComponentManager.Instance.CreateID();
            List<IComponent> anotherList = new List<IComponent>
            {
                new ModelComponent(Content.Load<Model>("untitled")),
                new TransformComponent(new Vector3(250, 400, -500), new Vector3(5,5,5)),
            };
            ComponentManager.Instance.AddAllComponents(skyboxEntity, anotherList);
        }

        public void CreateChopper(GraphicsDevice Device)
        {
            int ChopperEnt = ComponentManager.Instance.CreateID();
            ModelComponent mcp = new ModelComponent(Content.Load<Model>("Lab Models/Chopper"));
            Matrix[] meshWorldMatrices = new Matrix[3];
            meshWorldMatrices[0] = Matrix.CreateRotationY(0);
            meshWorldMatrices[1] = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            meshWorldMatrices[2] = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            mcp.MeshWorldMatrices = meshWorldMatrices;

            CameraComponent cmp = new CameraComponent(new Vector3(0, 100, 120), new Vector3(0, 500, 0), new Vector3(0, 1, 0), 10000.0f, 1.0f, Device.Viewport.AspectRatio);

            List<IComponent> ChopperComponentList = new List<IComponent>
            {
                //Skapa och lägg till alla komponenter som vi behöver för modellen
                mcp,
                new TransformComponent(new Vector3(0, 0, 0), new Vector3(1, 1, 1)),
                cmp,
                new ChaseCamComponent
                {
                    OffSet = new Vector3(0, 10, 35),
                    // sätt isDrunk till true om man vill ha en "drunk" kamera
                    IsDrunk = false
                },
                new ChopperComponent()
            };
            var keýComp = new KeyBoardComponent();
            keýComp.KeyBoardActions.Add("Forward", Keys.Up);
            keýComp.KeyBoardActions.Add("Backward", Keys.Down);
            keýComp.KeyBoardActions.Add("Right", Keys.Right);
            keýComp.KeyBoardActions.Add("Left", Keys.Left);
            keýComp.KeyBoardActions.Add("RotatenegativeX", Keys.D);
            keýComp.KeyBoardActions.Add("RotateX", Keys.A);
            keýComp.KeyBoardActions.Add("RotatenegativeY", Keys.W);
            keýComp.KeyBoardActions.Add("RotateY", Keys.S);
            keýComp.KeyBoardActions.Add("RotateZ", Keys.Q);
            keýComp.KeyBoardActions.Add("RotatenegativeZ", Keys.E);
            ComponentManager.Instance.AddAllComponents(ChopperEnt, ChopperComponentList);
            ComponentManager.Instance.AddComponentToEntity(ChopperEnt, keýComp);
            TryingShaders(ChopperEnt, cmp);
        }
        
        public void CreateManyTrees(HeightmapComponentTexture hmp, int width, int height, VertexPositionNormalTexture[] terrainVertices)
        {
            int treeEntity;

            List<IComponent> components;
            ModelComponent mcp;
            Model tree = Content.Load<Model>("Lab Models/Leaf_Oak");
            Texture2D treeGreen = Content.Load<Texture2D>("Lab Models/TexturesGreen");
            Texture2D treePurple = Content.Load<Texture2D>("Lab Models/TexturesSnor");

            int printedTrees = 0;
            Random random = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float terrainHeight = hmp.HeightMapData[x, y];

                    if ((terrainHeight > 8) && (terrainHeight < 14))
                    {
                        
                        float flatness = Vector3.Dot(terrainVertices[x + y * width].Normal, new Vector3(0, 1, 0));
                        float minFlatness = (float)Math.Cos(MathHelper.ToRadians(15));
                        if (flatness > minFlatness)
                        {
                            if (printedTrees <= 100)
                            {
                                float rand1 = (float)random.Next(1000) / 1000.0f;
                                float rand2 = (float)random.Next(1000) / 1000.0f;

                                float randomScale = (float)(random.Next(1, 5)/50.0f);


                                if (printedTrees % 2 == 0)
                                {
                                    mcp = new ModelComponent(tree, treeGreen);
                                }
                                else
                                    mcp = new ModelComponent(tree, treePurple);

                                treeEntity = ComponentManager.Instance.CreateID();
                                ShaderComponent shc = new ShaderComponent(shader);
                                shc.RealisticSettings();
                                components = new List<IComponent>(){
                                    shc,
                                    mcp,
                                    new TransformComponent(new Vector3((float)x - rand1, hmp.HeightMapData[x, y], -(float)y - rand2), new Vector3(randomScale,randomScale,randomScale))
                                };
                                ComponentManager.Instance.AddAllComponents(treeEntity, components);
                                printedTrees++;
                                x += 10;
                                if (x > width)
                                    x = 0;
                            }
                        }
                    }
                }
            }
        }

        public void CreateHumanoidEntity(Texture2D texture, GraphicsDevice graphics)
        {
            var humanoidEntity = ComponentManager.Instance.CreateID();

            var effect = new BasicEffect(graphics)
            {
                TextureEnabled = true,
                Texture = texture
            };
            List<IComponent> componentList = new List<IComponent>()
            {
                new HumanoidComponent(new Body(graphics, humanoidEntity), effect),
                new TransformComponent(new Vector3(10, 50, -10), new Vector3(1, 1, 1)),
                new CameraComponent(new Vector3(0, 100, 120), new Vector3(0, 500, 0), new Vector3(0, 1, 0), 10000.0f, 1.0f, graphics.Viewport.AspectRatio),
                new ChaseCamComponent
                {
                    OffSet = new Vector3(0, 10, 35),
                    IsDrunk = false
                },

            };
            ComponentManager.Instance.AddAllComponents(humanoidEntity, componentList);
        }

        public void CreateFreeRoamCam(Vector3 position, GraphicsDevice graphics)
        {
            var FreeRoamEnt = ComponentManager.Instance.CreateID();

            List<IComponent> componentList = new List<IComponent>()
            {
                new CameraComponent(position, new Vector3(0, 50, 0), Vector3.Up, 10000.0f, 1.0f, graphics.Viewport.AspectRatio),
                new FreeRoamCamComponent(){
                    LookAtOffSet = new Vector3(0, 10, -35),
                },
                new TransformComponent(position, Vector3.One),

            };
            var keýComp = new KeyBoardComponent();
            keýComp.KeyBoardActions.Add("Forward", Keys.Up);
            keýComp.KeyBoardActions.Add("Backward", Keys.Down);
            keýComp.KeyBoardActions.Add("Right", Keys.Right);
            keýComp.KeyBoardActions.Add("Left", Keys.Left);
            keýComp.KeyBoardActions.Add("RotatenegativeX", Keys.D);
            keýComp.KeyBoardActions.Add("RotateX", Keys.A);
            keýComp.KeyBoardActions.Add("RotatenegativeY", Keys.W);
            keýComp.KeyBoardActions.Add("RotateY", Keys.S);
            keýComp.KeyBoardActions.Add("RotateZ", Keys.Q);
            keýComp.KeyBoardActions.Add("RotatenegativeZ", Keys.E);
            keýComp.KeyBoardActions.Add("Up", Keys.Space);
            keýComp.KeyBoardActions.Add("Down", Keys.LeftShift);

            componentList.Add(keýComp);

            ComponentManager.Instance.AddAllComponents(FreeRoamEnt, componentList);
        }
        private void TryingShaders(int Entityid, CameraComponent cmp)
        {
            Effect ChopperEffect = Content.Load<Effect>("TestShaders");
            ComponentManager.Instance.AddComponentToEntity(Entityid, new ShaderComponent(ChopperEffect));

        }

        public void CreateHangar()
        {
            
            int EntityId = ComponentManager.Instance.CreateID();
            Model hangar = Content.Load<Model>("Lab Models/Chopper");
            //Model hangar = Content.Load<Model>("Lab3Stuff/moffett-old-building-b");
            ModelComponent mcp = new ModelComponent(hangar);
            TransformComponent tcp = new TransformComponent(new Vector3(20, 10, 20), new Vector3(10, 10, 10));
            ShaderComponent shc = new ShaderComponent(shader);
            shc.RealisticSettings();

            List<IComponent> componentList = new List<IComponent>()
            {
                shc,
                mcp,
                tcp,
            };

            ComponentManager.Instance.AddAllComponents(EntityId, componentList);
        }
        
        public void CreateFloor()
        {
            int EntityId = ComponentManager.Instance.CreateID();
            Model grid = Content.Load<Model>("ShadowStuff/grid");
            ModelComponent mcp = new ModelComponent(grid);
            TransformComponent tcp = new TransformComponent(Vector3.Zero, new Vector3(10, 10, 10));
            ShaderComponent shc = new ShaderComponent(shader);
            shc.RealisticSettings();

            List<IComponent> componentList = new List<IComponent>()
            {
                shc,
                mcp,
                tcp,
            };

            ComponentManager.Instance.AddAllComponents(EntityId, componentList);
        }

        public void CreateManyModels(int width, int height, int numberOfModels)
        {
            int modelEntity;

            List<IComponent> components;
            ModelComponent mcp;
            Model model = Content.Load<Model>("ShadowStuff/dude");

            int addedModels = 0;
            Random random = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (addedModels < numberOfModels)
                    {
                        float rand1 = (float)random.Next(1000);
                        float rand2 = (float)random.Next(1000);
                        
                        mcp = new ModelComponent(model);

                        modelEntity = ComponentManager.Instance.CreateID();
                        ShaderComponent shc = new ShaderComponent(shader);
                        shc.RealisticSettings();
                        components = new List<IComponent>(){
                            shc,
                            mcp,
                            new TransformComponent(new Vector3((float)x - rand1, 1, -(float)y - rand2), new Vector3(3,3,3))
                        };
                        ComponentManager.Instance.AddAllComponents(modelEntity, components);
                        addedModels++;
                        x += 100;
                        if (x > width)
                            x = 0;
                    }
                      
                }
            }
        }

    }
}
