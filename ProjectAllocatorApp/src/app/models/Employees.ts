import { Allocation } from "./Allocation"
import { JobRole } from "./JobRole.model"

export interface Employees {
    employeeId: number,
    employeeName: string,
    emailId: string,
    benchStartDate: string,
    benchEndDate: string,
    jobRoleId: number,
    jobRole: JobRole,
    typeId: number,
    allocation: Allocation,
    skills: null,
    trainingId? : number,
    internalProjectId?: number
}
