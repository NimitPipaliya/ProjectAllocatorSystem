import { JobRole } from "./JobRole"

export interface EditEmployee {
    employeeId:number,
    employeeName: string,
    emailId: string,
    benchStartDate: string,
    benchEndDate: string,
    jobRoleId: number,
    typeId:number,
    skills: string[]
}
