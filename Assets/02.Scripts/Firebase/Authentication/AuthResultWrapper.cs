using Firebase.Auth;

/// <summary>
/// Firebase 인증 결과(성공 여부, 메시지, 사용자 정보)를 담는 래퍼 클래스입니다.
/// </summary>
public class AuthResultWrapper
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public FirebaseUser User { get; set; }
    
    public AuthResultWrapper(bool isSuccess, string message, FirebaseUser user = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        User = user;
    }
}
