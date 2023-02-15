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
        public Author Author { get; internal set; }

        [JsonPropertyName("mentioned")] // mentioned may contain one or more multiple. Each user is a key value pair of UserMentioned in the dictionary.
        [JsonInclude]
        // NOTE: UserMentioned is a derived class of GroupMentioned. It inherits all properties from GrouplUser and has the Email property
        // that GroupMentioned does not have.
        public Dictionary<string, UserMentioned> Mentioned { get; set; } = new Dictionary<string, UserMentioned>();

        [JsonPropertyName("text")]
        [JsonInclude]
        public string Text { get; internal set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; internal set; }

        [JsonPropertyName("lastUpdatedTime")]
        [JsonInclude]
        public DateTime? LastUpdatedTime { get; internal set; }


        //----------------------------------------------------------------------------
        //
        // Comment.GetTextWithMentionedDisplayNames
        //
        // Replace the user ID in the text with the user's Display Name
        // Ex: Comment Text is: "Al Held should work. @[usr1w2fdcVFbu7ug3]  good morning @[UgpBw4fdcVFbu5ug6] I thinks that's it."
        // The output Text will be: "al Held should work. Ngoc Nicholas  good morning Foo Bars I thinks that's it."
        // Each mentioned is a Dictionary. In the example above, The Dictionary contains two
        // <Key, Value> pairs representing 2 UserMentioned objects.
        //
        //----------------------------------------------------------------------------
        public string GetTextWithMentionedDisplayNames(Dictionary<string, UserMentioned> mentioned)
        {
           string commentText = Text;
           if (mentioned != null)   // Comment has any Mentioned?
           {
                bool hasSingleUser = false;
                foreach (KeyValuePair<string, UserMentioned> usrOrUgp in mentioned)
                {
                    if (!string.IsNullOrEmpty((string)usrOrUgp.Value.Email))        // Group user does not have Email.
                    {
                        hasSingleUser = true;
                        break;
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


        //----------------------------------------------------------------------------
        //
        // Comment.ReplaceUserOrGroupIdWithDisplayName
        //
        //
        //----------------------------------------------------------------------------
        private string ReplaceUserOrGroupIdWithDisplayName(Regex pattern, string text, Dictionary<string, UserMentioned> mentioned)
        {
            var matches = pattern.Matches(text);
            string commentText = text;
            string value;
            int index;

            // Loop from the end count because the commentText keeps changing.
            for (int count = matches.Count - 1; count >= 0; count--)
            {
                value = matches[count].Value;                    // @[UgpBw4fdcVFbu5ug6]
                string key = value.Substring(2).TrimEnd(']');    // Get rid of "@[" at beginning and ']' at the end.The resulting string is the key for the dictionary of Mentioned objects.

                index = matches[count].Groups[0].Index;
                if (mentioned.ContainsKey(key))
                {
                    // Remove matched value @[UgpBw4fdcVFbu5ug6] in comment and replace it with the Display Name
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


    public class Author
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; internal set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }
    }


    public class GroupMentioned     // does not have an email
    {
        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }

        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("displayName")]
        [JsonInclude]
        public string DisplayName { get; internal set; }
    }


    public class UserMentioned : GroupMentioned
    {
        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; internal set; }
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
