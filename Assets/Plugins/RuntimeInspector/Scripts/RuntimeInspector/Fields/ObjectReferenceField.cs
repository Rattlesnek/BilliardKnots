using System;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using SFB;


namespace RuntimeInspectorNamespace
{
	public class ObjectReferenceField : InspectorField, IDropHandler
	{
#pragma warning disable 0649
		[SerializeField]
		private RectTransform referencePickerArea;

		[SerializeField]
		private PointerEventListener input;

		[SerializeField]
		private PointerEventListener inspectReferenceButton;
		private Image inspectReferenceImage;

		[SerializeField]
		protected Image background;

		[SerializeField]
		protected Text referenceNameText;
#pragma warning restore 0649

		public override void Initialize()
		{
			base.Initialize();

			input.PointerClick += OnPointerClick;

			if( inspectReferenceButton != null )
			{
				inspectReferenceButton.PointerClick += InspectReference;
				inspectReferenceImage = inspectReferenceButton.GetComponent<Image>();
			}
		}

		public override bool SupportsType( Type type )
		{
			return typeof( Object ).IsAssignableFrom( type );
		}

		// TODO
		private void OnPointerClick(PointerEventData eventData)
        {
            if (BoundVariableType == typeof(Mesh))
            {
				ShowMeshFileExplorer(eventData);
            }
            else
            {
				ShowReferencePicker(eventData);
            }
        }

		private GameObject CreateMeshObject(Mesh mesh, string objectName)
        {
			var go = new GameObject(objectName);
			go.AddComponent<MeshFilter>().mesh = mesh;
			go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
			return go;
		}

		private void ShowMeshFileExplorer(PointerEventData eventData)
        {
			var paths = StandaloneFileBrowser.OpenFilePanel("Open Mesh OBJ", "", "obj", false);
			if (paths.Length > 0)
			{
				var mesh = FastObjImporter.Instance.ImportFile(paths[0]);
				mesh.name = Path.GetFileName(paths[0]);
				OnReferenceChanged(mesh);
			}
		}

		private void ShowReferencePicker( PointerEventData eventData )
		{
			Object[] allReferences = Resources.FindObjectsOfTypeAll( BoundVariableType );

			ObjectReferencePicker.Instance.Skin = Inspector.Skin;
			ObjectReferencePicker.Instance.Show(
				( reference ) => OnReferenceChanged( (Object) reference ), null,
				( reference ) => (Object) reference ? ( (Object) reference ).name : "None",
				( reference ) => reference.GetNameWithType(),
				allReferences, (Object) Value, true, "Select " + BoundVariableType.Name, Inspector.Canvas );
		}

		private void InspectReference( PointerEventData eventData )
		{
			if( Value != null && !Value.Equals( null ) )
			{
				if( Value is Component )
					Inspector.Inspect( ( (Component) Value ).gameObject );
				else
					Inspector.Inspect( Value );
			}
		}

		protected override void OnBound( MemberInfo variable )
		{
			base.OnBound( variable );
			OnReferenceChanged( (Object) Value );
		}

		protected virtual void OnReferenceChanged( Object reference )
		{
			if( (Object) Value != reference )
				Value = reference;

			if( referenceNameText != null )
				referenceNameText.text = reference.GetNameWithType( BoundVariableType );

			if( inspectReferenceButton != null )
				inspectReferenceButton.gameObject.SetActive( Value != null && !Value.Equals( null ) );

			Inspector.RefreshDelayed();
		}

		public void OnDrop( PointerEventData eventData )
		{
			Object assignableObject = RuntimeInspectorUtils.GetAssignableObjectFromDraggedReferenceItem( eventData, BoundVariableType );
			if( assignableObject != null )
				OnReferenceChanged( assignableObject );
		}

		protected override void OnSkinChanged()
		{
			base.OnSkinChanged();

			background.color = Skin.InputFieldNormalBackgroundColor.Tint( 0.075f );

			referenceNameText.SetSkinInputFieldText( Skin );

			referenceNameText.resizeTextMinSize = Mathf.Max( 2, Skin.FontSize - 2 );
			referenceNameText.resizeTextMaxSize = Skin.FontSize;

			if( inspectReferenceImage )
			{
				inspectReferenceImage.color = Skin.TextColor.Tint( 0.1f );
				inspectReferenceImage.GetComponent<LayoutElement>().SetWidth( Mathf.Max( Skin.LineHeight - 8, 6 ) );
			}

			if( referencePickerArea )
			{
				Vector2 rightSideAnchorMin = new Vector2( Skin.LabelWidthPercentage, 0f );
				variableNameMask.rectTransform.anchorMin = rightSideAnchorMin;
				referencePickerArea.anchorMin = rightSideAnchorMin;
			}
		}

		public override void Refresh()
		{
			object lastValue = Value;
			base.Refresh();

			if( lastValue != Value )
				OnReferenceChanged( (Object) Value );
		}
	}
}