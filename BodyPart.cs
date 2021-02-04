using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class BodyPart : GameObject, IRenderable
    {
        public Vector2D NewPosition { get; set; }
        public GameObject ConnectedTo { get; set; }
        public char LookType { get; set; }

        public BodyPart(Player connectedTo, Vector2D position) : base("playerBody", position, ObjectType.BodyPart)
        {
            ConnectedTo = connectedTo;
            /* TODO
             * Change the bodypart looktype to something better
             */
            LookType = '©';
            NewPosition = ConnectedTo.GetPosition();
        }

        /// <summary>
        /// Change the current connected gameobject to a new connected gameobject
        /// </summary>
        /// <param name="newConnection">The new connected gameObject</param>
        public void NewConnection(GameObject newConnection)
        {
            ConnectedTo = newConnection;
        }
    }
}
