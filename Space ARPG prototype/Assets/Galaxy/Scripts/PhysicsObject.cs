using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public enum PhysicsObjectType { SMBH, STAR, ROCK_PLANET, ROCK_MOON, GAS_GIANT, G_G_MOON, GALAXY }
    public class PhysicsObject
    {
		public string name = "test";
        public rotate rotateScript;
        private List<PhysicsObject> components = new List<PhysicsObject>();

        public List<PhysicsObject> getComponents()
        {
            return components;
        }

        private PhysicsObject parent;
        public PhysicsObject Parent { get { return parent; } set { parent = value; } }

        private static int totalObjects = 0;
        public static int getNumObjects(){return totalObjects;}
        private int ID;

        private PhysicsObjectType physicsObjectType;
        public PhysicsObjectType ObjectType { get { return physicsObjectType; } set { physicsObjectType = value; } }

        public Vector2 Position, Velocity, Acceleration, NetForces, GravityWell;
        
        public float Angle, AngularVelocity, AngularAcceleration, Torque;

        private ulong mass, radius = 0;

        public double orbitalCircumference = 0, periodInSeconds = 0, linearVelocityScalar;

        private int numComponents = 0;

        private float timeFactor = 0.1f;

        public GameObject myGameObject;

        public int getNumComponents()
        {
            return numComponents;
        }

        public void addComponent(PhysicsObject component)
        {
            components.Add(component);
            numComponents++;
        }

        public void removeComponent(PhysicsObject component)
        {
            components.Remove(component);
            numComponents--;
        }

        public PhysicsObject pop()
        {
            if (components.Count > 0)
            {
                PhysicsObject tmp = components.First<PhysicsObject>();
                components.Remove(tmp);
                numComponents--;
                return tmp; 
            }
            else
            {
                return null;
            }
        }

        public PhysicsObject getComponent(int componentID)
        {
            return components[componentID];
        }

        //constructor with values
        public PhysicsObject(Vector2 _position, ulong _mass, PhysicsObjectType _type, float startingAngle)
        {
            Position = _position;
            mass = _mass;
            physicsObjectType = _type;
            Angle = startingAngle;
            finishSetup();
        }

        public PhysicsObject()
        {
            Position = Vector2.zero;
            mass = 0;
            physicsObjectType = PhysicsObjectType.SMBH;
            finishSetup();
        }

        public ulong calcRadius(PhysicsObject _gravityWell)
        {
            parent = _gravityWell;
            GravityWell = _gravityWell.Position;
            radius = (ulong)new Vector2(Position.x - GravityWell.x, Position.y - GravityWell.y).magnitude;
            orbitalCircumference = 2 * Math.PI * radius;
            calcPeriodInSeconds();
            return radius;
        }

        public double calcPeriodInSeconds()
        {

            periodInSeconds = Math.Sqrt(radius * radius * radius) * timeFactor ;

                calcTheRest();
            return periodInSeconds;
        }

        public double calcTheRest()
        {
            linearVelocityScalar = orbitalCircumference / periodInSeconds;

            Vector2 direction = new Vector2(Position.x - GravityWell.x, Position.y - GravityWell.y);
            Vector2 tanToDir = new Vector2(direction.y * -1, direction.x);
            tanToDir.Normalize();

            tanToDir.x *= (float)linearVelocityScalar;
            tanToDir.y *= (float)linearVelocityScalar;

            Velocity = tanToDir;

            return 0;
        }

        //calls any remaining setup functions that both constructors need to complete
        private void finishSetup()
        {
            Velocity = Vector2.zero;
            Acceleration = Vector2.zero;
            NetForces = Vector2.zero;
            AngularVelocity = 0.0f;
            AngularAcceleration = 0.0f;
            Torque = 0;
            ID = totalObjects++;
        }

        //print values every frame
        public void Update()
        {
            Physics();
        }

        private void Physics()
        {
            float DELTATIME = Time.deltaTime;
            //physics
            AngularVelocity = (float)(linearVelocityScalar / periodInSeconds);
            Angle += ((AngularVelocity * DELTATIME) + ((0.5f * AngularAcceleration) * (DELTATIME * DELTATIME)));

            if (Angle >= 360)
            {
                Angle = 0;
            }

            Position.x = (float)(GravityWell.x + radius * Math.Cos(Angle));
            Position.y = (float)(GravityWell.y + radius * Math.Sin(Angle));

            foreach (PhysicsObject physicsObj in components)
            {
                physicsObj.GravityWell.Set(Position.x, Position.y);
            }
        }

        public string Text()
        {
            string typeText;

            switch (physicsObjectType)
            {
                case PhysicsObjectType.SMBH:
                    typeText = "[0 - Supermassive Black Hole]";
                    break;
                case PhysicsObjectType.STAR:
                    typeText = "[1 - Star]";
                    break;
                case PhysicsObjectType.ROCK_PLANET:
                    typeText = "[2 - Rock Planet]";
                    break;
                case PhysicsObjectType.ROCK_MOON:
                    typeText = "[3 - Moon Orbitting Rock Planet]";
                    break;
                case PhysicsObjectType.GAS_GIANT:
                    typeText = "[4 - Gas Giant]";
                    break;
                case PhysicsObjectType.G_G_MOON:
                    typeText = "[5 - Moon Orbitting Gas Giant]";
                    break;
                default:
                    typeText = "[? - ????]";
                    break;
            }
            return "";// typeText + ID + " pos: " + position.Text() + " mass: " + mass + " radius: " + radius + " orbital period: " + periodInSeconds + " vel: " + velocity.Text() + " angle: " + angle + " angular Velocity: " + angularVelocity + "\n";
        }
    }
