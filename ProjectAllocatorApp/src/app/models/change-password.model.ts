export interface ChangePassword{
    loginId: string | undefined | null,
    oldPassword: string,
    newPassword: string,
    confirmNewPassword: string
}