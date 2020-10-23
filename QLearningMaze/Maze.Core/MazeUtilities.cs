namespace QLearningMaze.Core
{
    using Newtonsoft.Json;
    using System.IO;
    using Mazes;

    public class MazeUtilities
    {
        public static void SaveMaze(string saveToPath, object objectToSave)
        {
            string json = JsonConvert.SerializeObject(objectToSave);
            File.WriteAllText(saveToPath, json);
        }

        public static UserDefinedMaze LoadMaze(string loadFromPath)
        {
            UserDefinedMaze loaded = JsonConvert.DeserializeObject<UserDefinedMaze>(File.ReadAllText(loadFromPath));

            AddObstructions(loaded);

            return loaded;
        }

        private static void AddObstructions(UserDefinedMaze maze)
        {
            foreach (var obstruction in maze.Obstructions)
            {
                maze.AddWall(obstruction.BetweenSpace, obstruction.AndSpace);
            }
        }
    }
}
