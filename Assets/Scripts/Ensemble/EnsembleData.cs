using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensemble;

public class EnsembleData : MonoBehaviour
{
    public EnsembleAPI ensemble;

    // Start is called before the first frame update
    void Start()
    {
        ensemble = new EnsembleAPI();

        List<Predicate> structureFromFile = ensemble.loadSchemaFile("Assets/EnsembleData/schema.json");
        ensemble.loadSocialStructure(structureFromFile);

        List<Action> actions = ensemble.loadActionsFile("Assets/EnsembleData/actions.json");
        ensemble.addActions(actions, "actions.json", "actions.json");

        List<History> history = ensemble.loadHistoryFile("Assets/EnsembleData/history.json");
        ensemble.addHistory("history.json", history);

        Dictionary<string, Dictionary<string, string>> cast = ensemble.loadCastFile("Assets/EnsembleData/cast.json");
        ensemble.addCharacters(cast);

        List<Rule> triggerRules = ensemble.loadTriggerRulesFile("Assets/EnsembleData/triggerRules.json");
        ensemble.addRules("trigger", "triggerRules.json", triggerRules);

        List<Rule> volitionRules = ensemble.loadVolitionRulesFile("Assets/EnsembleData/volitionRules.json");
        ensemble.addRules("volition", "volitionRules.json", volitionRules);

        List<string> characters = ensemble.getCharacters();

        foreach (string character in characters)
        {
            Debug.Log(character);
        }
    }

    
}
