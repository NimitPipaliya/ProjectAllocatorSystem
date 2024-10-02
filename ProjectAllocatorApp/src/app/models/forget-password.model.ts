export interface ForgetPassword{
    loginId: string,
    securityQuestionId: number,
    answer: string,
    newPassword: string,
    confirmNewPassword: string
}