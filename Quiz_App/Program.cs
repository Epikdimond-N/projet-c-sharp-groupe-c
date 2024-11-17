using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Program
{
    static void Main()
    {
        string filePath = "questions.json";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Erreur : Le fichier 'questions.json' est introuvable.");
            return;
        }

        List<Question>? questions;

        try
        {
            string jsonData = File.ReadAllText(filePath);
            questions = JsonSerializer.Deserialize<List<Question>>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des questions : {ex.Message}");
            return;
        }

        if (questions == null || questions.Count == 0)
        {
            Console.WriteLine("Erreur : Aucune question trouvée dans le fichier.");
            return;
        }

        int score = 0;

        Console.WriteLine("Bienvenue au Quiz ! Répondez correctement pour continuer, sinon le jeu s'arrête.\n");

        foreach (var question in questions.OrderBy(q => Guid.NewGuid())) // Mélanger les questions
        {
            Console.WriteLine(question.QuestionText);

            for (int i = 0; i < question.Choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Choices[i]}");
            }

            int userChoice;
            while (true)
            {
                Console.Write("\nVotre réponse (entrez le numéro) : ");
                if (int.TryParse(Console.ReadLine(), out userChoice) &&
                    userChoice >= 1 && userChoice <= question.Choices.Count)
                {
                    break;
                }
                Console.WriteLine("Veuillez entrer un numéro valide.");
            }

            if (question.Choices[userChoice - 1].Equals(question.Answer, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Correct !\n");
                score++;
            }
            else
            {
                Console.WriteLine($"Faux ! La bonne réponse était : {question.Answer}.\n");
                Console.WriteLine($"Quiz terminé ! Votre score : {score}");
                return; // Arrêter le jeu immédiatement
            }
        }

        Console.WriteLine($"Bravo ! Vous avez terminé toutes les questions avec un score de {score}/{questions.Count}.");
    }

    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Choices { get; set; } = new List<string>();
        public string Answer { get; set; } = string.Empty;
    }
}
