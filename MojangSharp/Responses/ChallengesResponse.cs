using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MojangSharp.Responses
{
    /// <summary>
    /// Contains the security questions and their answer's id
    /// </summary>
    public class ChallengesResponse : Response
    {
        internal ChallengesResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// A challenge contains an ID to the answer, and ID to the question and the text of said question.
        /// </summary>
        public struct Challenge
        {
            public long AnswerID;
            public long QuestionID;
            public string QuestionText;
        }

        public static Challenge Parse(JToken json)
        {
            JObject challenge = JObject.Parse(json.ToString());
            return new Challenge()
            {
                AnswerID = challenge["answer"]["id"].Value<long>(),
                QuestionID = challenge["question"]["id"].Value<long>(),
                QuestionText = challenge["question"]["question"].Value<string>()
            };
        }

        /// <summary>
        /// Contains the challenges.
        /// </summary>
        public List<Challenge> Challenges { get; internal set; }
    }
}