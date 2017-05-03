namespace SMS.EPA.Repositories
{
    using System;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;

    public class FlowSettingRepository : IFlowSettingRepository
    {
        public IEnumerable<FlowModel> GetFlowSettingByFlowTemplateId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<FlowModel> epaFlow = db.Query<FlowModel, FlowTemplateModel, EvaluatorInfoViewModel, FlowModel>(
                   (f, ft, e) =>
                   {
                       f.FlowTemplate = ft;
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "FlowTemplateId, EmployeeId",
                   criteria.GetSpParams());

                return epaFlow;
            }
        }

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(ICriteria criteria)
        {
            IEnumerable<DepartmentFlowModel> flows;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    flows = multi.Read<DepartmentFlowModel>();
                    IEnumerable<ProcessFlowModel> processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByDepartment(flows, processes);
                }
            }

            return flows;
        }

        public IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(ICriteria criteria)
        {
            IEnumerable<PersonalFlowModel> flows;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    flows = multi.Read<PersonalFlowModel, Group, PersonalFlowModel>(
                        (flow, gr) =>
                        {
                            flow.Group = gr;

                            return flow;
                        },
                        "GroupId");

                    IEnumerable<ProcessFlowModel> processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByEmployee(flows, processes);
                }
            }

            return flows;
        }

        private static void GroupProcessFlowByDepartment(IEnumerable<DepartmentFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            if (flows == null || processes == null)
            {
                return;
            }

            foreach (DepartmentFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.DepartmentId.Equals(flow.DepartmentId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        case (int)ProcessType.Management:
                            flow.Managements.Add(process);
                            break;

                        default:
                            throw new ArgumentException("type not in case");
                    }
                }
            }
        }

        private static void GroupProcessFlowByEmployee(IEnumerable<PersonalFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (PersonalFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.EmployeeId.Equals(flow.EmployeeId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        case (int)ProcessType.Management:
                            flow.Managements.Add(process);
                            break;

                        default:
                            throw new ArgumentException("type not in case");
                    }
                }
            }
        }
    }
}