export interface RegisterUser{
    name: string,
    loginId: string,
    gender : string,
    email: string,
    phone: string,
    password: string,
    confirmPassword: string,
    securityQuestionId : number,
    answer : string,
    userRoleId : number
    }