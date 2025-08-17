using System;
using Firebase.Auth;
using UnityEngine;

public class AuthenticationManager : BehaviourSingleton<AuthenticationManager>
{
    private IAuthenticator _authenticator;

    private FirebaseUser _user;
    public FirebaseUser User => _user;
    public Action<string> OnAuthenticated;

    public event Action OnLogin;
    private void Awake()
    {
        _authenticator = new FirebaseAuthenticator();
        DontDestroyOnLoad(gameObject);
    }

    public async void CreateAccountAsync(string email, string password, string passwordConfirm)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
        {
            OnAuthenticated?.Invoke("이메일, 비밀번호, 비밀번호 확인을 모두 입력해주세요.");
            return;
        }

        if (password != passwordConfirm)
        {
            OnAuthenticated?.Invoke("비밀번호와 비밀번호 확인이 일치하지 않습니다.");
            return;
        }
     
        AuthResultWrapper result = await _authenticator.CreateAccountAsync(email, password);
        
        OnAuthenticated?.Invoke(result.Message);
        
        if (result.IsSuccess)
        {
            _user = result.User;
            Debug.Log("회원가입 성공");
            // Scene 전환
        }
    }
    
    public async void SignInAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            OnAuthenticated?.Invoke("이메일과 비밀번호를 모두 입력해주세요.");
            return;
        }

        AuthResultWrapper result = await _authenticator.SignInAsync(email, password);
        
        OnAuthenticated?.Invoke(result.Message);
        
        if (result.IsSuccess)
        {
            _user = result.User;
            Debug.Log("로그인 성공");
            OnLogin?.Invoke();
            // Scene 전환
        }
    }
}