using UnityEngine;


namespace CellBig.UI.Event
{
	public class AddDialogMsg : Message
	{
	}

	public class RemoveDialogMsg : Message
	{
	}

	public class ShowDialogMsg : Message
	{
	}

	public class HideDialogMsg : Message
	{
	}

	public class DialogEnterClickMsg : Message
	{
	}

	public class DialogCloseClickMsg : Message
	{
	}


	public class ToggleTabChangeMsg : Message
	{
		// Control을 사용하지 않는 탭에서 사용, "컨텐츠 이름"을 키값으로 사용.
		public int tabIndex;
	}

#region Control
	//Control을 위한 메시지들. 다른 곳에서 사용 금지.

	public class TabControlClickMsg : Message
	{
		
		public int index;
	}

	public class CommonButtonClickMsg : Message
	{
		public int index;
	}

	public class CommonButtonActiveMsg : Message
	{
		public bool active;
	}

	//슬라이더의 값이 변경 되었을 때 호출.
	public class CommomUISliderValueChangeMsg : Message
	{
		public float value;
	}

	// 유저 입력된 텍스트를 넘긴다.
	public class SendInputTextMsg : Message
	{
		public string inputText;
	}

#endregion
}
