using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Base class for HASS operations providing common functionality
    /// </summary>
    public abstract class HassBaseOperation
    {
        /// <summary>
        /// Operation ID for tracking
        /// </summary>
        public string OperationId { get; protected set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Operation name
        /// </summary>
        public abstract string OperationName { get; }

        /// <summary>
        /// Current status of the operation
        /// </summary>
        public OperationStatus Status { get; protected set; } = OperationStatus.NotStarted;

        /// <summary>
        /// Last error message if any
        /// </summary>
        public string? LastError { get; protected set; }

        /// <summary>
        /// Start time of the operation
        /// </summary>
        public DateTime? StartTime { get; protected set; }

        /// <summary>
        /// End time of the operation
        /// </summary>
        public DateTime? EndTime { get; protected set; }

        /// <summary>
        /// Execute the operation
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool Execute()
        {
            try
            {
                StartTime = DateTime.Now;
                Status = OperationStatus.Running;
                LoggingService.LogInfo($"Starting operation: {OperationName} [{OperationId}]");

                bool result = ExecuteInternal();

                EndTime = DateTime.Now;
                Status = result ? OperationStatus.Completed : OperationStatus.Failed;
                
                var duration = EndTime - StartTime;
                LoggingService.LogInfo($"Operation {OperationName} {(result ? "completed" : "failed")} in {duration?.TotalMilliseconds:F0}ms");

                return result;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Status = OperationStatus.Failed;
                EndTime = DateTime.Now;
                LoggingService.LogError($"Operation {OperationName} failed with exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reset the operation to initial state
        /// </summary>
        public virtual void Reset()
        {
            Status = OperationStatus.NotStarted;
            LastError = null;
            StartTime = null;
            EndTime = null;
            LoggingService.LogInfo($"Operation {OperationName} reset");
        }

        /// <summary>
        /// Get operation summary
        /// </summary>
        /// <returns>Operation summary string</returns>
        public virtual string GetSummary()
        {
            var duration = (EndTime ?? DateTime.Now) - (StartTime ?? DateTime.Now);
            return $"{OperationName} [{OperationId}] - Status: {Status}, Duration: {duration.TotalMilliseconds:F0}ms";
        }

        /// <summary>
        /// Internal execution method to be implemented by derived classes
        /// </summary>
        /// <returns>True if successful</returns>
        protected abstract bool ExecuteInternal();

        /// <summary>
        /// Validate operation parameters
        /// </summary>
        /// <returns>True if valid</returns>
        protected virtual bool ValidateParameters()
        {
            return true;
        }

        /// <summary>
        /// Log operation progress
        /// </summary>
        /// <param name="message">Progress message</param>
        protected void LogProgress(string message)
        {
            LoggingService.LogInfo($"{OperationName}: {message}");
        }

        /// <summary>
        /// Log operation error
        /// </summary>
        /// <param name="message">Error message</param>
        protected void LogError(string message)
        {
            LastError = message;
            LoggingService.LogError($"{OperationName}: {message}");
        }
    }

    /// <summary>
    /// Operation status enumeration
    /// </summary>
    public enum OperationStatus
    {
        NotStarted,
        Running,
        Completed,
        Failed,
        Cancelled
    }
}