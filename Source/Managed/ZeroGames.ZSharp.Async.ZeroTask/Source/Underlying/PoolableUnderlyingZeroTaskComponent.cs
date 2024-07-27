﻿// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

/// <summary>
/// Encapsulates generic logic for a poolable underlying task.
/// Similar to ManualResetValueTaskSourceCore.
/// </summary>
public struct PoolableUnderlyingZeroTaskComponent<TResult>
{

	public void Initialize()
	{
		_isCompleted = false;
	}

	public void Deinitialize()
	{
		// Invalidate token immediately.
		Token = Token.Next;
		
		// Release reference to these so that they can get GCed earlier.
		_continuation = null;
		_stateMachine = null;
		_error = null;
	}

	public EUnderlyingZeroTaskStatus GetStatus(UnderlyingZeroTaskToken token)
	{
		ValidateToken(token);

		if (!_isCompleted || (_continuation is null && _stateMachine is null))
		{
			return EUnderlyingZeroTaskStatus.Pending;
		}

		if (_error is null)
		{
			return EUnderlyingZeroTaskStatus.Succeeded;
		}

		return _error.SourceException is LifecycleExpiredException || _error.SourceException is OperationCanceledException ? EUnderlyingZeroTaskStatus.Canceled : EUnderlyingZeroTaskStatus.Faulted;
	}

	public TResult GetResult(UnderlyingZeroTaskToken token)
	{
		ValidateToken(token);

		if (!_isCompleted)
		{
			throw new InvalidOperationException();
		}
		
		_error?.Throw();
		return _result;
	}
	
	public void SetStateMachine(IAsyncStateMachine stateMachine, UnderlyingZeroTaskToken token)
	{
		ValidateToken(token);
		ValidateContinuation();
		_stateMachine = stateMachine;
	}

	public void SetContinuation(Action continuation, UnderlyingZeroTaskToken token)
	{
		ValidateToken(token);
		ValidateContinuation();
		_continuation = continuation;
	}

	public void SetResult(TResult result)
	{
		_result = result;
		SignalCompletion();
	}

	public void SetException(Exception exception)
	{
		_error = ExceptionDispatchInfo.Capture(exception);
		SignalCompletion();
	}
	
	public UnderlyingZeroTaskToken Token { get; private set; }

	private void ValidateToken(UnderlyingZeroTaskToken token)
	{
		if (token != Token)
		{
			throw new InvalidOperationException();
		}
	}

	private void ValidateContinuation()
	{
		if (_continuation is not null || _stateMachine is not null)
		{
			throw new InvalidOperationException();
		}
	}

	private void SignalCompletion()
	{
		if (_isCompleted)
		{
			throw new InvalidOperationException();
		}

		_isCompleted = true;

		if (_stateMachine is not null)
		{
			_stateMachine.MoveNext();
		}
		else
		{
			_continuation!.Invoke();
		}
	}

	private bool _isCompleted;
	private IAsyncStateMachine? _stateMachine;
	private Action? _continuation;
	private TResult _result;
	private ExceptionDispatchInfo? _error;

}