import { Allocation } from "./Allocation"
import { JobRole } from "./JobRole"

export interface Employee {
    employeeId: number,
    employeeName: string,
    emailId: string,
    benchStartDate: string,
    benchEndDate: string,
    jobRoleId: number,
    jobRole: JobRole,
    typeId: number,
    allocation: Allocation,
    skills: string[]
}
