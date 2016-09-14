namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class FlowSettingRepository : IFlowSettingRepository
    {
        public IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<FlowModel> epaFlow = db.Query<FlowModel, FlowTemplateModel, Employee, FlowModel>(
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

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(ICriteria criteria)
        {
            IEnumerable<DepartmentFlowModel> flows = new List<DepartmentFlowModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    flows = multi.Read<DepartmentFlowModel>();
                    IEnumerable<ProcessFlowModel> processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByDepartment(flows, processes);
                }
            }

            return flows;
        }

        public IEnumerable<PersonalFlowModel> GetPersonalFlow(ICriteria criteria)
        {
            IEnumerable<PersonalFlowModel> flows = new List<PersonalFlowModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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

        private void GroupProcessFlowByDepartment(IEnumerable<DepartmentFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            if (flows != null && processes != null)
            {
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
                        }
                    }
                }
            }
        }

        private void GroupProcessFlowByEmployee(IEnumerable<PersonalFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
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
                    }
                }
            }
        }
    }
}