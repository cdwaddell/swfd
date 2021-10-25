using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using swfd.core;

namespace swfd.console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    /*
                    Object Expected
                    {
                        SubscriptionId = "00000000-0000-0000-0000-000000000000"
                        SubscriptionDeploymentSpnName = ""
                        GeneralClusterIdentityName = ""
                        PCIClusterIdentityName = ""
                        PHIClusterIdentityName = ""
                        GeneralClusterGroupName = ""
                        PCIClusterGroupName = ""
                        PHIClusterGroupName = ""
                        GeneralClusterResourceGroupName = ""
                        PCIClusterResourceGroupName = ""
                        PHIClusterResourceGroupName = ""
                        NetworkingResourceGroupName = ""
                        StrongDMResourceGroupName = ""
                        StrongDMSPN = ""
                        CoreLOBWorkspaceName = ""
                        AKSLOBWorkspaceName = "" 
                        LOBFoundationRepoName = ""
                        GeneralAKSClusterName = ""
                        PCIAKSClusterName = ""
                        PHIAKSClusterName = ""
                    }

                    Scripts Required:
                    Excution scripts retun "true" for "false" then additional parameters for the input object
                        OnboardQualys.ps1
                        CreateSPN.ps1
                        CreateADGroup.ps1
                        AssignRBAC.ps1
                        CreateRG.ps1
                        ?RegisterIPAM.ps1?
                        CreateWorkspace.ps1
                        CreateGitRepo.ps1
                        AddWorkspaceToRepo.ps1
                        DeployWorkspace.ps1
                        ?OnboardToHashiVault.ps1?
                        ?OnboardToGSLB.ps1?
                        ?OnboardToAkamai.ps1?
                    
                    Validation scripts return "true" or "false
                        OnboardQualys.validate.ps1
                        CreateSPN.validate.ps1
                        CreateADGroup.validate.ps1
                        AssignRBAC.validate.ps1
                        CreateRG.validate.ps1
                        ?RegisterIPAM.validate.ps1?
                        CreateWorkspace.validate.ps1
                        CreateGitRepo.validate.ps1
                        AddWorkspaceToRepo.validate.ps1
                        DeployWorkspace.validate.ps1
                        ?OnboardToHashiVault.validate.ps1?
                        ?OnboardToGSLB.validate.ps1?
                        ?OnboardToAkamai.validate.ps1?
                    */
                    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    services.AddTransient<ScriptRunningService>();
                    services.AddSwfd()
                        .AddWorkflow("Something Else", Path.Combine(appData, "swfd/scripts"), Path.Combine(appData, "swfd/inputs"))
                        .AddWorkflow("LOB Onboarding", Path.Combine(appData, "swfd/scripts"), Path.Combine(appData, "swfd/inputs"))
                            .AddSteps(
                                new WorkflowStep("OnboardQualys", x => x.SubscriptionId)
                                ,new WorkflowStep("CreateSPN", x => x.SubscriptionDeploymentSpnName)
                                //new WorkflowStep("CreateCIs") //This is uncertain
                                ,new WorkflowStep("CreateSPN", (x, ps) => x.GeneralSPNName = ps[0], x => x.GeneralClusterIdentityName)
                                ,new WorkflowStep("CreateSPN", (x, ps) => x.PCISPNName = ps[0], x => x.PCIClusterIdentityName)
                                ,new WorkflowStep("CreateSPN", (x, ps) => x.PHISPNName = ps[0], x => x.PHIClusterIdentityName)
                                ,new WorkflowStep("CreateRG", x => x.SubscriptionId, x => x.GeneralClusterResourceGroupName, x => x.Region)
                                ,new WorkflowStep("CreateRG", x => x.SubscriptionId, x => x.PCIClusterResourceGroupName, x => x.Region)
                                ,new WorkflowStep("CreateRG", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName, x => x.Region)
                                ,new WorkflowStep("CreateRG", x => x.SubscriptionId, x => x.NetworkingResourceGroupName, x => x.Region)
                            ).AddSteps(
                                new WorkflowStep("CreateADGroup", (x, ps) => x.GeneralADGroup = ps[0], x => x.GeneralClusterGroupName, x => x.GeneralClusterIdentityName)
                                ,new WorkflowStep("CreateADGroup", (x, ps) => x.PCIADGroup = ps[0], x => x.PCIClusterGroupName, x => x.PCIClusterIdentityName)
                                ,new WorkflowStep("CreateADGroup", (x, ps) => x.PHIADGroup = ps[0], x => x.PHIClusterGroupName, x => x.PHIClusterIdentityName)
                                ,new WorkflowStep("AssignRBAC", x => x.SubscriptionDeploymentSpnName, x => "Contributor", x => x.SubscriptionId)
                                //,new WorkflowStep("CreateRG", x => x.SubscriptionId, x => x.StrongDMResourceGroupName, x => x.Region)
                            ).AddSteps(
                                new WorkflowStep("AssignRBAC", x => x.GeneralADGroup, x => "AcrPull", x => x.SubscriptionId, x => x.GeneralClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.GeneralADGroup, x => "Monitoring Metrics Publisher", x => x.SubscriptionId, x => x.GeneralClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.GeneralADGroup, x => "Network Contributor", x => x.SubscriptionId, x => x.GeneralClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.GeneralADGroup, x => "Storage Account Contributor", x => x.SubscriptionId, x => x.GeneralClusterResourceGroupName)

                                ,new WorkflowStep("AssignRBAC", x => x.PCIADGroup, x => "AcrPull", x => x.SubscriptionId, x => x.PCIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PCIADGroup, x => "Monitoring Metrics Publisher", x => x.SubscriptionId, x => x.PCIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PCIADGroup, x => "Network Contributor", x => x.SubscriptionId, x => x.PCIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PCIADGroup, x => "Storage Account Contributor", x => x.SubscriptionId, x => x.PCIClusterResourceGroupName)

                                ,new WorkflowStep("AssignRBAC", x => x.PHIADGroup, x => "AcrPull", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PHIADGroup, x => "Monitoring Metrics Publisher", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PHIADGroup, x => "Network Contributor", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName)
                                ,new WorkflowStep("AssignRBAC", x => x.PHIADGroup, x => "Storage Account Contributor", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName)
                                
                                //,new WorkflowStep("AssignRBAC", x => x.StrongDMSPN, x => "Humana Contributor??", x => x.SubscriptionId, x => x.PHIClusterResourceGroupName)
                                //new WorkflowStep("RegisterIPAM", (x, ps) => x.CIDRRange = ps[0]) //This is uncertain

                                ,new WorkflowStep("CreateWorkspace", x => x.CoreLOBWorkspaceName, x => x.CPTAdminGroup)
                                ,new WorkflowStep("CreateWorkspace", x => x.AKSLOBWorkspaceName, x => x.CPTAdminGroup)
                                ,new WorkflowStep("CreateGitRepo", x => x.LOBFoundationRepoName, x => x.CPTAdminGroup)
                            ).AddSteps(
                                new WorkflowStep("AddWorkspaceToRepo", x => x.CoreLOBWorkspaceName)
                                ,new WorkflowStep("AddWorkspaceToRepo", x => x.AKSLOBWorkspaceName)
                            ).AddSteps(
                                new WorkflowStep("DeployWorkspace", x => x.CoreLOBWorkspaceName)
                                ,new WorkflowStep("DeployWorkspace", x => x.AKSLOBWorkspaceName)
                            ).AddSteps(
                                new WorkflowStep("OnboardToDynatrace", x => x.GeneralAKSClusterName)
                                ,new WorkflowStep("OnboardToDynatrace", x => x.PCIAKSClusterName)
                                ,new WorkflowStep("OnboardToDynatrace", x => x.PHIAKSClusterName)

                                // new WorkflowStep("OnboardToHashiVault", x => x.GeneralAKSClusterName)
                                // ,new WorkflowStep("OnboardToHashiVault", x => x.PCIAKSClusterName)
                                // ,new WorkflowStep("OnboardToHashiVault", x => x.PHIAKSClusterName)
                                
                                // ,new WorkflowStep("OnboardToGSLB", x => x.GeneralAKSClusterName)
                                // ,new WorkflowStep("OnboardToGSLB", x => x.PCIAKSClusterName)
                                // ,new WorkflowStep("OnboardToGSLB", x => x.PHIAKSClusterName)
                                
                                // ,new WorkflowStep("OnboardToAkamai", x => x.GeneralAKSClusterName)
                                // ,new WorkflowStep("OnboardToAkamai", x => x.PCIAKSClusterName)
                                // ,new WorkflowStep("OnboardToAkamai", x => x.PHIAKSClusterName)
                            );
                });
    }
}
