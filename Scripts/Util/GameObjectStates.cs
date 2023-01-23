using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class GameObjectStates
    {
        private GameObject[] gameObjects;

        public GameObjectStates(params GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void Show(GameObject showGameObject)
        {
            var length = gameObjects.Length;
            for (var i = 0; i < length; i++)
            {
                var gameobject = gameObjects[i];
                gameobject.SetActive(showGameObject == gameobject);
            }
        }
    }
}