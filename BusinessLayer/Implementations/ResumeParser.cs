using System.Text.RegularExpressions;
using BusinessLayer.DTOs;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace BusinessLayer.Implementations
{
    public static class ResumeParser
    {
        public static ResumeParseResultDto Parse(byte[] fileBytes, string fileName)
        {
            string text = "";

            if (fileName.EndsWith(".pdf"))
            {
                using var pdf = PdfDocument.Open(fileBytes);
                foreach (var page in pdf.GetPages())
                    text += page.Text + " ";
            }
            else if (fileName.EndsWith(".docx"))
            {
                using var doc = WordprocessingDocument.Open(new MemoryStream(fileBytes), false);
                text = doc.MainDocumentPart!.Document.Body!.InnerText;
            }

            return ExtractFields(text);
        }

        private static ResumeParseResultDto ExtractFields(string text)
        {
            var result = new ResumeParseResultDto();

            // 📧 Email
            var emailMatch = Regex.Match(text, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            if (emailMatch.Success)
                result.Email = emailMatch.Value;

            // 📞 Mobile
            var phoneMatch = Regex.Match(text, @"\b[6-9]\d{9}\b");
            if (phoneMatch.Success)
                result.Mobile = phoneMatch.Value;

            // 👤 Name (first 2 words heuristic)
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length >= 2)
            {
                result.FirstName = Capitalize(words[0]);
                result.LastName = Capitalize(words[1]);
            }

            // 🧠 Skills (simple keyword scan)
            string[] skillsDb = { "C#", "ASP.NET", "Angular", "MySQL", "SQL","Java", "Python", "React", "Node", "HTML", "CSS" };
            var foundSkills = skillsDb.Where(s => text.Contains(s, StringComparison.OrdinalIgnoreCase));
            result.Skills = string.Join(", ", foundSkills);

            // 📍 Location
            var locationMatch = Regex.Match(text, @"(Hyderabad|Bangalore|Chennai|Delhi|Mumbai|Pune)", RegexOptions.IgnoreCase);
            if (locationMatch.Success)
                result.Location = locationMatch.Value;

            return result;
        }

        private static string Capitalize(string s)
            => char.ToUpper(s[0]) + s.Substring(1).ToLower();
    }
}
