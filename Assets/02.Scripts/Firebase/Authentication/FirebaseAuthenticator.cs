using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System;

public class FirebaseAuthenticator : IAuthenticator
{
    public async Task<AuthResultWrapper> CreateAccountAsync(string email, string password)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsInitialized)
        {
            Debug.LogError("Firebase is not initialized. Please check FirebaseManager.");
            return new AuthResultWrapper(isSuccess: false, message: "연결에 실패했습니다. 잠시 후 다시 시도해주세요.");
        }
        
        FirebaseAuth auth = FirebaseManager.Instance.Auth;

        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            return new AuthResultWrapper(isSuccess: true, message: "회원가입에 성공했습니다.", user: result.User);
        }
        catch (Exception e)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + e);
            return new AuthResultWrapper(isSuccess: false, message: GetFirebaseAuthErrorMessage(e));
        }
    }

    public async Task<AuthResultWrapper> SignInAsync(string email, string password)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsInitialized)
        {
            Debug.LogError("Firebase is not initialized. Please check FirebaseManager.");
            return new AuthResultWrapper(isSuccess: false, message: "연결에 실패했습니다. 잠시 후 다시 시도해주세요.");
        }
        
        FirebaseAuth auth = FirebaseManager.Instance.Auth;

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            return new AuthResultWrapper(isSuccess: true, message: "로그인에 성공했습니다.", user: result.User);
            
        }
        catch (Exception e)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + e);
            return new AuthResultWrapper(isSuccess: false, message: GetFirebaseAuthErrorMessage(e));
        }
    }

    private string GetFirebaseAuthErrorMessage(Exception exception)
    {
        if (exception.GetBaseException() is FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "이미 사용 중인 이메일입니다.";
                case AuthError.WrongPassword:
                    return "비밀번호가 틀렸습니다.";
                case AuthError.InvalidEmail:
                    return "유효하지 않은 이메일 형식입니다.";
                case AuthError.UserNotFound:
                    return "존재하지 않는 계정입니다.";
                case AuthError.WeakPassword:
                    return "보안 수준이 낮은 비밀번호입니다.";
                default:
                    return "알 수 없는 오류가 발생했습니다.";
            }
        }
        return "인증 처리 중 오류가 발생했습니다.";
    }
}
