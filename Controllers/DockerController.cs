using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;

namespace auto_deploy_docker.Controllers
{
    [Route("api/[controller]")]
    public class DockerController : Controller
    {
        [HttpPost("Deploy")]
        public IActionResult Deploy([FromBody] ParamDeployDocker param)
        {
            var file_path = Path.Combine(Environment.CurrentDirectory, "file_build");
            //var file_path = Path.Combine("D:/", "file_build");

            string git_clone = $"{param.git_clone} {file_path}";
            string images_name = $"{param.images_name}";
            string internal_http_port = $"{param.internal_http_port}";
            string external_http_port = $"{param.external_http_port}";
            string internal_ssl_port = $"{param.external_http_port}";
            string external_ssl_port = $"{param.external_ssl_port}";

            string strStop = $"docker stop {images_name}";
            string strRmi = $"docker rmi {images_name}";
            string strBuild = $"docker build -t {images_name} {file_path}";
            string strRun = $"docker run -d --rm -p {external_http_port}:{internal_http_port} -p {external_ssl_port}:{internal_ssl_port} --name {images_name} {images_name}";

            var returnObject = new ClsReturnResult();
            var env = Environment.OSVersion;

            try
            {
                if (env.Platform == PlatformID.Win32NT)
                {
                    using (var ps = PowerShell.Create())
                    {
                        var gitClone = ps.AddScript(git_clone).Invoke();
                        var dockerStop = ps.AddScript(strStop).Invoke();
                        var dockerRmi = ps.AddScript(strRmi).Invoke();
                        var dockerBuild = ps.AddScript(strBuild).Invoke();
                        var dockerRun = ps.AddScript(strRun).Invoke();
                    }
                }
                else if (env.Platform == PlatformID.Unix)
                {
                    var gitCloneProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"{git_clone}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    gitCloneProcess.Start();
                    gitCloneProcess.WaitForExit();

                    var dockerStopProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"{strStop}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    dockerStopProcess.Start();
                    dockerStopProcess.WaitForExit();

                    var dockerRmiProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"{strRmi}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    dockerRmiProcess.Start();
                    dockerRmiProcess.WaitForExit();

                    var dockerBuildProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"{strBuild}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    dockerBuildProcess.Start();
                    dockerBuildProcess.WaitForExit();

                    var dockerRunProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"{strRun}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    dockerRunProcess.Start();
                    dockerRunProcess.WaitForExit();
                }

                returnObject.statusCode = 200;
                returnObject.message = $"Deploy Docker images: {images_name} success.";
            }
            catch (Exception ex)
            {
                returnObject.statusCode = 500;
                returnObject.message = $"Deploy Docker images: {images_name} fail.";
                returnObject.exception = ex.Message;
            }
            finally
            {
                if (env.Platform == PlatformID.Win32NT)
                {
                    using (var ps = PowerShell.Create())
                    {
                        var removeItem = ps.AddScript($"Remove-Item {file_path} -Recurse -Force -Verbose").Invoke();
                    }
                }
                else if (env.Platform == PlatformID.Unix)
                {
                    var removeItemProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $" -c \"rm -rf {file_path}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    removeItemProcess.Start();
                    removeItemProcess.WaitForExit();
                }
            }

            return StatusCode(returnObject.statusCode, returnObject);
        }
    }

    public class ClsReturnResult
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public string exception { get; set; }
    }

    public class ParamDeployDocker
    {
        public string git_clone { get; set; }
        public string images_name { get; set; }
        public string internal_http_port { get; set; }
        public string external_http_port { get; set; }
        public string internal_ssl_port { get; set; }
        public string external_ssl_port { get; set; }
    }

}
