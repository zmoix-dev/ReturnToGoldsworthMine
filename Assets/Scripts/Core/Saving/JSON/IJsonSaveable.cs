using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RPG.Saving {
    public interface IJsonSaveable
    {
        JToken CaptureAsJToken();
        void RestoreFromJToken(JToken state);
    }
}