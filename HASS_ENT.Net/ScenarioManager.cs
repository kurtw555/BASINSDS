using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Scenario management for HASS modeling workflows
    /// Handles creation, modification, and execution of modeling scenarios
    /// </summary>
    public class ScenarioManager : HassBaseOperation
    {
        public override string OperationName => "Scenario Management";

        private readonly List<Scenario> _scenarios = new();
        private Scenario? _activeScenario;

        /// <summary>
        /// Create a new scenario
        /// </summary>
        /// <param name="name">Scenario name</param>
        /// <param name="description">Scenario description</param>
        /// <returns>Created scenario</returns>
        public Scenario CreateScenario(string name, string description = "")
        {
            var scenario = new Scenario
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                CreatedDate = DateTime.Now,
                Status = ScenarioStatus.Created
            };
            
            _scenarios.Add(scenario);
            LogProgress($"Created scenario: {name}");
            
            return scenario;
        }

        /// <summary>
        /// Set the active scenario
        /// </summary>
        /// <param name="scenarioId">Scenario ID</param>
        /// <returns>True if successful</returns>
        public bool SetActiveScenario(string scenarioId)
        {
            var scenario = _scenarios.FirstOrDefault(s => s.Id == scenarioId);
            if (scenario != null)
            {
                _activeScenario = scenario;
                LogProgress($"Set active scenario: {scenario.Name}");
                return true;
            }
            
            LogError($"Scenario not found: {scenarioId}");
            return false;
        }

        /// <summary>
        /// Get all scenarios
        /// </summary>
        /// <returns>List of scenarios</returns>
        public List<Scenario> GetScenarios()
        {
            return _scenarios.ToList();
        }

        /// <summary>
        /// Get active scenario
        /// </summary>
        /// <returns>Active scenario or null</returns>
        public Scenario? GetActiveScenario()
        {
            return _activeScenario;
        }

        protected override bool ExecuteInternal()
        {
            // Default execution - validate all scenarios
            LogProgress("Validating all scenarios");
            
            bool allValid = true;
            foreach (var scenario in _scenarios)
            {
                if (!ValidateScenario(scenario))
                {
                    allValid = false;
                    LogError($"Scenario validation failed: {scenario.Name}");
                }
            }
            
            return allValid;
        }

        private bool ValidateScenario(Scenario scenario)
        {
            // Basic validation
            return !string.IsNullOrEmpty(scenario.Name) && 
                   !string.IsNullOrEmpty(scenario.Id);
        }
    }

    /// <summary>
    /// Scenario data structure
    /// </summary>
    public class Scenario
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ScenarioStatus Status { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public Dictionary<string, string> FilePaths { get; set; } = new();
        public List<string> DataSources { get; set; } = new();
    }

    /// <summary>
    /// Scenario status enumeration
    /// </summary>
    public enum ScenarioStatus
    {
        Created,
        Configured,
        Validated,
        Running,
        Completed,
        Failed
    }
}