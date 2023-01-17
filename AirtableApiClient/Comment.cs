using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AirtableApiClient
{
    public class Comment
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("author")]
        [JsonInclude]
        public Author Author { get; set; }

        [JsonPropertyName("mentioned")] // mentioned may contain one or more multiple. Each user is a key value pair of UserMentioned in the dictionary.
        [JsonInclude]
        public Dictionary<string, UserMentioned> Mentioned { get; set; } = new Dictionary<string, UserMentioned>();

        [JsonPropertyName("text")]
        [JsonInclude]
        public string Text { get; set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; internal set; }

        [JsonPropertyName("lastUpdatedTime")]
        [JsonInclude]
        public DateTime? LastUpdatedTime { get; internal set; }

        public string GetTextWithMentionedDisplayNames(Dictionary<string, UserMentioned> mentioned)
        {
           string commentText = Text;
           if (mentioned != null)   // Comment has any Mentioned?
           {
                bool hasSingleUser = false;
                foreach (KeyValuePair<string, UserMentioned> usrOrUgp in mentioned)
                {
                    if (!string.IsNullOrEmpty((string)usrOrUgp.Value.Email))
                    {
                        hasSingleUser = true;
                    }
                }

                Regex pattern = null;
                if (hasSingleUser)
                {
                    pattern = new Regex("@\\[usr[a-zA-Z0-9]{14}\\]");
                    commentText = ReplaceUserOrGroupIdWithDisplayName(pattern, commentText, mentioned);
                }
                // Assume that it also has Group user(s)
                pattern = new Regex("@\\[ugp[a-zA-Z0-9]{14}\\]");
                commentText = ReplaceUserOrGroupIdWithDisplayName(pattern, commentText, mentioned);

            }
            return commentText;
        }
        private string ReplaceUserOrGroupIdWithDisplayName(Regex pattern, string text, Dictionary<string, UserMentioned> mentioned)
        {
            var matches = pattern.Matches(text);
            string commentText = text;
            string value;
            int index;

            for (int count = matches.Count - 1; count >= 0; count--)
            {
                value = matches[count].Value;
                string key = value.Substring(2).TrimEnd(']');    // Get rid of "@[" at beginning and ']' at the end

                index = matches[count].Groups[0].Index;
                if (mentioned.ContainsKey(key))
                {
                    commentText = commentText.Remove(index, value.Length).Insert(index, mentioned[key].DisplayName);
                }
                else
                {
                    return null;
                }
            }
            return commentText;
        }

    }   // end Comment


    public class CommentData
    {
        public string text { get; set; }
    }

    public class Author
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }
    }


    public class UserGroupMentioned     // does not have an email
    {
        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("displayName")]
        [JsonInclude]
        public string DisplayName { get; set; }
    }


    public class UserMentioned : UserGroupMentioned
    {
        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; set; }
    }


    public class CommentList
    {
        [JsonPropertyName("comments")]
        [JsonInclude]
        public List<Comment> Comments { get; internal set; }

        [JsonPropertyName("offset")]
        [JsonInclude]
        public string Offset { get; internal set; }
    }

}   // end namespace
