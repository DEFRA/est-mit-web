﻿namespace EST.MIT.Web.Helpers;

public class Validatable : IValidatable
{
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
    public string ErrorPath { get; set; }
    public virtual Dictionary<string, List<string>> AddErrors(Dictionary<string, List<string>> errors)
    {
        Errors.Clear();
        foreach (var error in errors)
        {
            if (error.Key.StartsWith(ErrorPath))
            {
                string errorKey = error.Key.Remove(0, ErrorPath.Length);
                if (!errorKey.Contains('.') &&
                    !Errors.ContainsKey(errorKey))
                {
                    Errors.Add(errorKey, error.Value);
                }
            }
        }
        return Errors;
    }
}