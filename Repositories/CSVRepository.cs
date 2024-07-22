using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace TodoList.Repositories
{
    internal class CSVRepository : IRepository<TodoModel>
    {
        private const string PATH = ".\\todo.csv";
        private void EnsureFile()
        {
            if (!File.Exists(PATH))
            {
                string headers = "ID,Title,Date,IsDone";
                File.WriteAllText(PATH, headers + Environment.NewLine);
            }
        }
        public TodoModel Add(TodoModel todo)
        {
            EnsureFile();
            // Generate ID for the new TodoModel
            todo.Id = GenerateId();
            // Append the new TodoModel to the CSV file
            string csvLine = $"{todo.Id},{todo.Title},{todo.XmlDate},{todo.IsDone}";
            File.AppendAllText(PATH, csvLine + Environment.NewLine);
            return todo;
        }
        public void DeleteById(int id)
        {
            EnsureFile();
            // Read all lines except the header
            List<string> lines = File.ReadAllLines(PATH).Skip(1).ToList();
            // Find and remove the line with the matching ID
            lines.RemoveAll(line => int.Parse(line.Split(',')[0]) == id);
            // Rewrite the CSV file with the updated lines
            File.WriteAllText(PATH, "ID,Title,Date,IsDone" + Environment.NewLine);
            File.AppendAllLines(PATH, lines);
        }
        public int GenerateId()
        {
            EnsureFile();
            // Read all lines except the header
            List<string> lines = File.ReadAllLines(PATH).Skip(1).ToList();
            // Generate the next ID based on existing IDs
            if (lines.Count == 0)
            {
                return 1; // Start with ID 1 if no entries exist
            }
            else
            {
                int maxId = lines.Select(line => int.Parse(line.Split(',')[0])).Max();
                return maxId + 1;
            }
        }
        public List<TodoModel> GetAll()
        {
            EnsureFile();
            List<TodoModel> todos = new List<TodoModel>();
            // Read all lines except the header
            List<string> lines = File.ReadAllLines(PATH).Skip(1).ToList();
            foreach (string line in lines)
            {
                string[] fields = line.Split(',');
                TodoModel todo = new TodoModel
                {
                    Id = int.Parse(fields[0]),
                    Title = fields[1],
                    XmlDate = fields[2],
                    IsDone = bool.Parse(fields[3])
                };
                todos.Add(todo);
            }
            return todos;
        }
        public List<TodoModel> GetAll(Func<TodoModel, bool> predicate)
        {
            EnsureFile();
            List<TodoModel> todos = GetAll();
            return todos.Where(predicate).ToList();
        }
        public TodoModel GetById(int id)
        {
            EnsureFile();
            // Read all lines except the header
            List<string> lines = File.ReadAllLines(PATH).Skip(1).ToList();
            // Find the line with the matching ID
            string line = lines.FirstOrDefault(l => int.Parse(l.Split(',')[0]) == id);
            if (line != null)
            {
                string[] fields = line.Split(',');
                TodoModel todo = new TodoModel
                {
                    Id = int.Parse(fields[0]),
                    Title = fields[1],
                    XmlDate = fields[2],
                    IsDone = bool.Parse(fields[3])
                };
                return todo;
            }
            else
            {
                return null;
            }
        }
        public TodoModel Update(TodoModel todo)
        {
            EnsureFile();
            // Read all lines except the header
            List<string> lines = File.ReadAllLines(PATH).Skip(1).ToList();
            // Find and update the line with the matching ID
            for (int i = 0; i < lines.Count; i++)
            {
                string[] fields = lines[i].Split(',');
                int id = int.Parse(fields[0]);
                if (id == todo.Id)
                {
                    lines[i] = $"{todo.Id},{todo.Title},{todo.XmlDate},{todo.IsDone}";
                    break;
                }
            }
            // Rewrite the CSV file with the updated lines
            File.WriteAllText(PATH, "ID,Title,Date,IsDone" + Environment.NewLine);
            File.AppendAllLines(PATH, lines);
            return todo;
        }
    }
}