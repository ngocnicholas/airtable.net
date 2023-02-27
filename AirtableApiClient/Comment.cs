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
        public Dictionary<string, MentionedEntity> Mentioned { get; set; } = new Dictionary<string, MentionedEntity>();

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
        // Ex: Comment Text is: "Al Held should work. @[usr1w2fdcVFbu7ug3]  good morning @[ugpBw4fdcVFbu5ug6] I think that's it."
        // The output Text will be: "Al Held should work. Ngoc Nicholas  good morning Foo Bars I think that's it."
        // Each mentioned is a Dictionary. In the example above, The Dictionary contains two
        // <Key, Value> pairs representing 2 UserMentioned objects.
        //
        //----------------------------------------------------------------------------
        public string GetTextWithMentionedDisplayNames()
        {
           if (Mentioned != null)   // Comment has any Mentioned?
           {
                string pattern = "@\\[(usr|ugp)[a-zA-Z0-9]{14}\\]";

                // Remove matched value which is a mentioned such as @[ugpBw4fdcVFbu5ug6] in comment and replace it with the Display Name.
                return Regex.Replace(Text, pattern, match => FindDisplayName(match));
            }
            return Text;
        }


        //----------------------------------------------------------------------------
        //
        // Comment.FindDisplayName
        // This method is called as many times as matches that Regex.Replace() finds in this Comment.
        // Each match is a User ID or Group ID. The corresponding Display Name will be returned.
        //
        //----------------------------------------------------------------------------
        private string FindDisplayName(Match match)
        {
            string value = match.Value;                      // i.e. @[ugpBw4fdcVFbu5ug6]
            string key = value.Substring(2).TrimEnd(']');    // Get rid of "@[" at beginning and ']' at the end.The resulting string is the key for the dictionary of Mentioned objects.
            if (Mentioned.ContainsKey(key))
            {
                return Mentioned[key].DisplayName;
            }
            return value;
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


    public class MentionedEntity
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

        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; internal set; }
    }


    public class CommentList
    {
        [JsonPropertyName("comments")]
        [JsonInclude]
        public Comment[] Comments { get; internal set; }


        [JsonPropertyName("offset")]
        [JsonInclude]
        public string Offset { get; internal set; }
    }

}   // end namespace
