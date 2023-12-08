using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using ImGuiNET;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Locator;
using MoreLinq;
using Newtonsoft.Json;
using Serilog;

namespace ExileCore.Shared {
    public class PluginCompiler : IDisposable {
        private PluginCompiler() {
        }

        public static PluginCompiler Create() {
            if (!PluginCompiler.IsEnabled) {
                return null;
            }
            return new PluginCompiler();
        }

        public static bool IsEnabled {
            get {
                return MSBuildLocator.IsRegistered;
            }
        }

        public static PluginCompiler CreateOrThrow() {
            PluginCompiler pluginCompiler = PluginCompiler.Create();
            if (pluginCompiler == null) {
                throw new Exception("Plugin compilation is disabled");
            }
            return pluginCompiler;
        }

        public void CompilePlugin(FileInfo csProj, string outputDirectory) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["OutputPath"] = outputDirectory;
            dictionary["exapiPackage"] = AppDomain.CurrentDomain.BaseDirectory;
            dictionary["RuntimeIdentifier"] = "win-x64";
            dictionary["SelfContained"] = "False";
            dictionary["PathMap"] = "";
            Dictionary<string, string> dictionary2 = dictionary;
            string text = csProj.Name.Replace(csProj.Extension, "");
            ProjectPropertyElement projectPropertyElement = null;
            try {
                ProjectRootElement projectRootElement = ProjectRootElement.Open(csProj.FullName);
                PluginCompiler.PatchProject(projectRootElement);
                if (projectRootElement.HasUnsavedChanges) {
                    projectRootElement.Save();
                }
                projectPropertyElement = projectRootElement.Properties.FirstOrDefault((ProjectPropertyElement x) => x.Name == "TargetFramework");
                PluginCompiler.BuildSemaphore.Wait();
                try {
                    using (ProjectCollection projectCollection = new ProjectCollection()) {
                        MsBuildLogger msBuildLogger = new MsBuildLogger();
                        BuildParameters buildParameters = new BuildParameters(projectCollection) {
                            DisableInProcNode = true,
                            EnableNodeReuse = true,
                            Loggers = (IEnumerable<Microsoft.Build.Framework.ILogger>)(new MsBuildLogger[] { msBuildLogger })
                        };
                        ProjectInstance projectInstance = ProjectInstance.FromProjectRootElement(projectRootElement, new ProjectOptions {
                            GlobalProperties = dictionary2
                        });
                        BuildResult buildResult = this.buildManager.Build(buildParameters, new BuildRequestData(projectInstance, new string[] { "Restore", "Build" }, null));
                        if (buildResult.OverallResult != null) {
                            throw buildResult.Exception ?? new Exception("Build failed:\n" + string.Join<BuildError>("\n", msBuildLogger.Errors));
                        }
                        projectCollection.UnloadAllProjects();
                    }
                }
                finally {
                    PluginCompiler.BuildSemaphore.Release();
                }
            }
            catch (Exception ex) {
                if (projectPropertyElement == null || projectPropertyElement.Value == null) {
                    DebugWindow.LogError(text + " -> CompilePlugin failed, but you can try running the fix_plugins.ps1 script", 10f);
                }
                else if (projectPropertyElement.Value == "net4.8") {
                    DebugWindow.LogError(text + " -> CompilePlugin failed, but you can try updating its TargetFramework to net8.0-windows", 10f);
                }
                else {
                    DebugWindow.LogError(text + " -> CompilePlugin failed", 2f);
                }
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                defaultInterpolatedStringHandler.AppendFormatted(text);
                defaultInterpolatedStringHandler.AppendLiteral(" -> ");
                defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
                DebugWindow.LogError(defaultInterpolatedStringHandler.ToStringAndClear(), 2f);
                throw;
            }
        }

        private static void PatchProject(ProjectRootElement pre) {
            PluginCompiler.ReplacePackageVersion(pre, "ImGui.NET", typeof(ImGui), 4);
            PluginCompiler.ReplacePackageVersion(pre, "morelinq", typeof(MoreEnumerable), 3);
            PluginCompiler.ReplacePackageVersion(pre, "Serilog", typeof(Log), 3);
            PluginCompiler.ReplacePackageVersion(pre, "Newtonsoft.Json", typeof(JsonSerializer), 3);
            ProjectPropertyElement projectPropertyElement = pre.Properties.FirstOrDefault((ProjectPropertyElement x) => x.Name == "TargetFramework");
            string text = ((projectPropertyElement != null) ? projectPropertyElement.Value : null);
            if (text != null && !text.StartsWith("net4") && text != "net8.0-windows") {
                projectPropertyElement.Value = "net8.0-windows";
            }
        }

        private static void ReplacePackageVersion(ProjectRootElement pre, string packageName, Type typeFromPackage, int versionParts) {
            ProjectItemElement projectItemElement = pre.Items.FirstOrDefault((ProjectItemElement x) => x.ItemType == "PackageReference" && x.Include.Equals(packageName, StringComparison.OrdinalIgnoreCase));
            if (projectItemElement != null) {
                ProjectMetadataElement projectMetadataElement = projectItemElement.Metadata.FirstOrDefault((ProjectMetadataElement x) => x.Name == "Version");
                if (projectMetadataElement != null) {
                    string text = Assembly.GetAssembly(typeFromPackage).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
                    Version version;
                    if (Version.TryParse(text, out version)) {
                        text = version.ToString(versionParts);
                    }
                    if (text.Trim() != projectMetadataElement.Value.Trim()) {
                        projectMetadataElement.Value = text;
                    }
                }
            }
        }

        public void Dispose() {
            this.buildManager.ResetCaches();
            this.buildManager.CancelAllSubmissions();
            this.buildManager.ShutdownAllNodes();
            this.buildManager.Dispose();
        }

        private static readonly SemaphoreSlim BuildSemaphore = new SemaphoreSlim(1, 1);

        private readonly BuildManager buildManager = new BuildManager("pluginCompiler");
    }
}
