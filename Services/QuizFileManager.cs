using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Services
{
    using Quiz.Model;
    using Quiz.Models;
    using System.IO;
    using System.Windows;

    public class QuizFileManager
    {
        public static void SaveQuizToFile(Quiz quiz)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Zapisz plik",
                // * DefaultExt = "txt"
            };


            if (saveFileDialog.ShowDialog() == true)
            {
                File.Delete(saveFileDialog.FileName);

                // Szyfrowanie danych AES
                var original = ConvertQuizToFileFormat(quiz);
                var publicKey = Convert.ToBase64String(CryptographyManager.GenerateRandomPublicKey());

                var encrypted = CryptographyManager.Encrypt(original, publicKey);

                // Сохраняем IV и зашифрованные данные в одной строке
                var combined = publicKey + "::" + encrypted;

                // Zapisanie zaszyfrowanych danych do pliku
                File.WriteAllText(saveFileDialog.FileName, combined);

                /*File.WriteAllText(saveFileDialog.FileName, $"Imię, Nazwisko, DataUrodzenia, Stanowisko, Wynagrodzenie, TypUmowy\n");
                // Zapis wszystkich pracowników do pliku
                foreach (Employee employee in _team.Employees)
                {
                    File.AppendAllText(saveFileDialog.FileName, $"{employee.FirstName}, {employee.LastName}, {employee.BirthDate.ToShortDateString()}, {employee.WorkPosition.Name}, {employee.Salary}, {employee.EmploymentContract.Type}\n");
                }*/

                MessageBox.Show("Zapisano plik", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static Quiz LoadQuizFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Otwórz plik"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // Odczytanie zaszyfrowanych danych z pliku
                var combined = File.ReadAllText(openFileDialog.FileName);
                var parts = combined.Split(new[] { "::" }, StringSplitOptions.None);

                if (parts.Length != 2)
                {
                    MessageBox.Show("Nieprawidłowy format pliku", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                var publicKey = parts[0];
                var encrypted = parts[1];

                // Odszyfrowanie danych AES
                var decrypted = CryptographyManager.Decrypt(encrypted, publicKey);
                // Deserializacja obiektu Quiz

                string[] lines = decrypted.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    MessageBox.Show("Nieprawidłowy format pliku", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                string title = lines[0].Trim().Remove(lines[0].Length - 1, 1);
                Quiz quiz = new Quiz(title);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts2 = lines[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    string questionText = parts2[0].Trim();
                    QuestionType questionType = (QuestionType)Enum.Parse(typeof(QuestionType), parts2[1].Trim());
                    Question question = new Question(questionText, questionType);

                    if (parts2.Length < 2 + 2 * Quiz.ANSWER_COUNT - 1)
                    {
                        MessageBox.Show("Nieprawidłowy format pliku", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                    for (int j = 2; j < 2 + 2 * Quiz.ANSWER_COUNT - 1; j += 2)
                    {
                        string answerText = parts2[j].Trim();
                        bool isCorrect = bool.Parse(parts2[j + 1].Trim());
                        question.Answers.Add(new Answer(answerText, isCorrect));
                    }
                    quiz.Questions.Add(question);
                }

                return quiz;
            }
            return null;
        }

        private static string ConvertQuizToFileFormat(Quiz quiz)
        {
            StringBuilder sb = new StringBuilder();
            string separator = ";";
            sb.AppendLine($"{quiz.Title}" + separator);
            foreach (var question in quiz.Questions)
            {
                sb.Append(question.QuestionText.ToString() + separator);
                sb.Append($"{question.Type.ToString()}" + separator);
                foreach (var answer in question.Answers)
                {
                    sb.Append(answer.AnswerText + separator)
                        .Append(answer.IsCorrect.ToString() + separator);
                }
                // Dodajemy pusty separator na końcu odpowiedzi
                sb.AppendLine();
            }

            MessageBox.Show(sb.ToString(), "Zawartość pliku", MessageBoxButton.OK, MessageBoxImage.Information);
            return sb.ToString();
        }
    }
}
