using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System;

namespace AppInsightsLoggingProviders
{
    public class http1
    {
        private readonly ILogger<http1> _logger;

        public http1(ILogger<http1> logger)
        {
            _logger = logger;
        }

        [Function("http1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                // Get the 3 numbers from query parameters
                string num1 = req.Query["num1"];
                string num2 = req.Query["num2"];
                string num3 = req.Query["num3"];

                //string num1 = "1";
                //string num2 = "1";
                //string num3 = "1";

                // Validate parameters
                if (string.IsNullOrEmpty(num1) || string.IsNullOrEmpty(num2) || string.IsNullOrEmpty(num3))
                {
                    return new BadRequestObjectResult("Please provide num1, num2, and num3 as query parameters");
                }

                // Set up the process start info
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "funccppapp.exe", // Assumes the C++ executable is named Adder.exe
                    Arguments = $"{num1} {num2} {num3}", // Pass the numbers as arguments
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetCurrentDirectory() // Set working directory to function root
                };

                // Start the process
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    // Read the output
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Wait for the process to exit
                    process.WaitForExit();

                    // Check if there was an error
                    if (process.ExitCode != 0)
                    {
                        _logger.LogError($"C++ app error: {error}");
                        return new BadRequestObjectResult($"Error executing calculation: {error}");
                    }

                    // Return the result
                    return new OkObjectResult(output.Trim());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


            // Program.NewMethod();

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
