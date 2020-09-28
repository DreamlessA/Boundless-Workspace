using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System;
using System.IO;
using System.Collections;
using TMPro;

public class WindowController : MonoBehaviour
{
    #region Private Variables
    [SerializeField, Tooltip("Background layer (an image, or just white) [required]")]
    private Image _background;

    [SerializeField, Tooltip("Text layer above the background [optional]")]
    private TextMeshProUGUI _text;

    [SerializeField, Tooltip("Drawing layer above the background and text [optional]")]
    private Image _drawing;

    [SerializeField, Tooltip("Bounding box")]
    private UIComponent _bounds;

    [SerializeField, Tooltip("The model of the 3D window")]
    private GameObject _model;

    [SerializeField, Tooltip("Enabling the editing feature of the window [requires the drawing layer]")]
    private bool _editable;

    private UIButton _editButton;
    private UIButton _closeButton;

    [SerializeField, Tooltip("Margin between elements")]
    private float _elementMargin;

    [SerializeField, Tooltip("Margin when palcing on a pysical plane")]
    private float _placementMargin;

    private Button _lastButtonHit;
    private Vector3 _offsetPosition;
    private float _offsetDistance;
    private bool _inEditMode;
    private bool _outOfBounds;
    private TextureManager _textureManager;
    private Collider _otherCollider;
    private Boolean triggerDown;

    private Menu _drawingMenu;

    #endregion // Private Variables

    private Menu getDrawingMenu()
    {
        if (_drawingMenu == null)
        {
            _drawingMenu = FindObjectOfType<MenuController>().menu;
        }
        return _drawingMenu;
    }

    public void SetText(string text)
    {
        if (_text != null)
        {
            this._text.text = text;
        }
    }

    // dimensions are in meters
    public static WindowController New2DWindow(float width, float height, float margin = 0f)
    {
        float marginHorizontal = margin;
        float marginVertical = margin;

        WindowController wc = Instantiate(Resources.Load<WindowController>("Prefabs/2DWindow"));

        wc._bounds.transform.localScale = new Vector3(width, height, 0.005f);
        wc._background.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        wc._text.GetComponent<RectTransform>().sizeDelta = new Vector2(width - marginHorizontal, height - marginVertical);
        wc._drawing.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        wc.AlignElements();

        return wc;
    }

    public static WindowController New3DWindow()
    {
        WindowController wc = Instantiate(Resources.Load<WindowController>("Prefabs/3DWindow"));

        return wc;
    }

    #region Public Methods
    //For 2Dwindow objects
    public void SetTexture(Texture2D texture)
    {
        this._background.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 0f));
    }

    //For 3Dwindow objects
    public void SetModel(AssetBundle assetBundle)
    {
        if (assetBundle == null)
        {
            Debug.Log("null bundle");
            return;
        }

        //	Load the model in the AssetBundle file
        var prefab = assetBundle.LoadAllAssets()[0];
        _model = Instantiate(prefab) as GameObject;
        print(assetBundle == null);
        print(_model == null);
        print(assetBundle.LoadAllAssets());

        _model.transform.parent = transform;
        _model.transform.position = new Vector3(0, 0, 0);
        _model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Bounds model_bounds = new Bounds();
        for (int i = 0; i < _model.transform.childCount; i++)
        {
            Renderer model_renderer = _model.transform.GetChild(i).GetComponent<Renderer>();
            if (model_renderer != null) {
                model_bounds.Encapsulate(model_renderer.bounds.max);
                model_bounds.Encapsulate(model_renderer.bounds.min);
            }
        }
    }
    #endregion // Public Methods

    #region Unity Methods
    private void Awake()
    {
        if (_bounds == null)
        {
            Debug.LogError("Error: WindowController._bounds is not set, disabling script.");
            enabled = false;
            return;
        }

        _closeButton = Instantiate(Resources.Load<UIButton>("Prefabs/CloseButton"));
        _closeButton.transform.parent = transform;

        if (_editable)
        {
            _editButton = Instantiate(Resources.Load<UIButton>("Prefabs/EditButton"));
            _editButton.transform.parent = transform;
            _editButton.OnControllerTriggerUp += Edit_Triggered;
            _editButton.OnIntersectionEnter += Collision_Enter;
            _bounds.OnIntersectionExit += Collision_Exit;
            if (_drawing == null)
            {
                Debug.LogError("Error: WindowController._drawing is not set but editable is True, disabling script.");
                enabled = false;
                return;
            }
        }

        //_closeButton.OnToggle += Close;
        _bounds.OnControllerTriggerDown += Drag_Start;
        _bounds.OnControllerDrag += Drag_Movement;
        _bounds.OnControllerTriggerUp += Drag_Stop;

        _bounds.OnIntersectionEnter += Collision_Enter;
        _bounds.OnIntersectionExit += Collision_Exit;

        
        _closeButton.OnControllerTriggerUp += Close_Triggered;
        _closeButton.OnIntersectionEnter += Collision_Enter;
        _closeButton.OnIntersectionExit += Collision_Exit;

        AlignElements();
    }

    private void Start()
    {
        if (_background != null)
        {
            Vector2 dimensions = _background.GetComponent<RectTransform>().sizeDelta;

            float width = dimensions.x;
            float height = dimensions.y;
            float aspectRatio = width / height;

            if (_editable)
            {
                //note taking & drawing
                _textureManager = new TextureManager(aspectRatio);
                _textureManager.SetBrushStroke(5);
                Texture2D tmp = _textureManager.GetTexture();
                _drawing.sprite = Sprite.Create(tmp, new Rect(0f, 0f, tmp.width, tmp.height), new Vector2(0f, 0f));
            }
        }

        EnableUI(true);
    }

    private void OnDestroy()
    {
        //_closeButton.OnToggle -= Close;
        _bounds.OnControllerTriggerDown -= Drag_Start;
        _bounds.OnControllerDrag -= Drag_Movement;
        _bounds.OnControllerTriggerUp -= Drag_Stop;

        _bounds.OnIntersectionEnter -= Collision_Enter;
        _closeButton.OnIntersectionEnter -= Collision_Enter;
        _editButton.OnIntersectionEnter -= Collision_Enter;
        _bounds.OnIntersectionExit -= Collision_Enter;
        _closeButton.OnIntersectionExit -= Collision_Enter;
        _editButton.OnIntersectionExit -= Collision_Enter;

        _closeButton.OnControllerTriggerUp -= Close_Triggered;

        if (_editable)
        {
            _editButton.OnControllerTriggerUp -= Edit_Triggered;
        }
    }

    

    private void Update()
    {
    }
    #endregion // Unity Methods

    #region Private Methods
    private void EnableUI(bool enabled)
    {
        _closeButton.enabled = enabled;
        if (_editable)
        {
            _editButton.enabled = enabled;
        }    
    }

    // assumes obj has localScale x to y aspect ratio of 1
    private void LoadTexture(GameObject obj, Texture2D texture)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("UI/Unlit/Transparent"));
        rend.material.mainTexture = texture;

        // perserve the original scale. This means we make it wider or narrower so that it has the
        // new aspect ratio.
        //float priorRatio = obj.transform.localScale.x / obj.transform.localScale.y;
        //float newRatio = (float)texture.width / texture.height;
        //obj.transform.localScale += new Vector3(obj.transform.localScale.x * (newRatio - priorRatio), 0, 0);

        AlignElements();
    }

    private void LoadMesh(GameObject obj, Mesh mesh)
    {
        obj.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AlignElements()
    {
        //assume the orientation of all elements are the same (default (0,0,0))
        //align based on the position of _bounds (default (0,0,0))
        _closeButton.transform.localPosition = _bounds.transform.localPosition;
        _closeButton.transform.localPosition += new Vector3((_bounds.transform.localScale.x + _closeButton.transform.localScale.x) / 2, 0, 0);
        _closeButton.transform.localPosition += new Vector3(0, (_bounds.transform.localScale.y - _closeButton.transform.localScale.y) / 2, 0);
        _closeButton.transform.localPosition += new Vector3(_elementMargin, -_elementMargin, 0);
        if (_editable)
        {
            _editButton.transform.localPosition = _closeButton.transform.localPosition;
            _editButton.transform.localPosition -= new Vector3(0, (_closeButton.transform.localScale.y + _editButton.transform.localScale.y) / 2, 0);
            _editButton.transform.localPosition += new Vector3(0, -_elementMargin, 0);
        }
    }

    private Vector2 Nearest2DPosition(Vector3 position)
    {
        Vector3 delta = position - transform.position;
        float x_val = Vector3.Dot(delta, transform.right);
        float y_val = Vector3.Dot(delta, transform.up);
        return new Vector2(x_val, y_val);
    }
    #endregion // Private Methods

    #region Event Handlers
    private void Close(bool flag)
    {
    }

    private Action<Vector2> ToolBeginPoint;
    private Action ToolPausePoint;
    private Action<Vector2> ToolResumePoint;
    private Action<Vector2> ToolContinuePoint;
    private Action ToolEndPoint;

    private void Drag_Start(MLInputController controller, float triggerValue, Vector3 dragPosition)
    {
        controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.High);
        controller.StopFeedbackPatternVibe();

        triggerDown = true;
        if (_inEditMode && _editable)
        {

            Vector2 uv = Nearest2DPositionUV(dragPosition);
            if (InBoundsUV(uv))
            {
                _outOfBounds = false;
                _textureManager.SetBrushColor(((ColorChoiceSection)getDrawingMenu().sections[0]).getCurrentColor());
                _textureManager.SetBrushStroke(((ToolSizeSection)getDrawingMenu().sections[2]).tool_size_current);
                switch (((ToolChoiceSection)getDrawingMenu().sections[1]).getCurrentToolName())
                {
                    case "brush":
                        _textureManager.UseBrush();
                        ToolBeginPoint = _textureManager.BeginPaint;
                        ToolPausePoint = _textureManager.PausePaint;
                        ToolResumePoint = _textureManager.ResumePaint;
                        ToolContinuePoint = _textureManager.ContinuePaint;
                        ToolEndPoint = _textureManager.EndPaint;
                        break;
                    case "eraser":
                        _textureManager.UseEraser();
                        ToolBeginPoint = _textureManager.BeginPaint;
                        ToolPausePoint = _textureManager.PausePaint;
                        ToolResumePoint = _textureManager.ResumePaint;
                        ToolContinuePoint = _textureManager.ContinuePaint;
                        ToolEndPoint = _textureManager.EndPaint;
                        break;
                    case "clear":
                        ToolBeginPoint = _ => _textureManager.Clear();
                        ToolPausePoint = null;
                        ToolResumePoint = null;
                        ToolContinuePoint = null;
                        ToolEndPoint = null;
                        break;
                }

                if (ToolBeginPoint != null) ToolBeginPoint.Invoke(uv);
            }
            else _outOfBounds = true;
        } else {
            if (_model != null)
            {
                Quaternion bounds_rotation = _bounds.transform.rotation;
                Quaternion original_rotation = _model.transform.rotation;
                transform.rotation = controller.Orientation;

                _model.transform.rotation = original_rotation;
                _bounds.transform.rotation = bounds_rotation;

                Vector3 delta = dragPosition - transform.position;
                _offsetPosition = new Vector3(Vector3.Dot(delta, this.transform.right), Vector3.Dot(delta, this.transform.up), Vector3.Dot(delta, this.transform.forward));
            } else
            {
                _offsetPosition = Nearest2DPosition(dragPosition);
            }

            _offsetDistance = (dragPosition - controller.Position).magnitude;
        }
    }

    private static bool InBounds(float x)
    {
        return 0.0f <= x && x < 1.0f;
    }

    private static bool InBoundsUV(Vector2 uv)
    {
        return InBounds(uv.x) && InBounds(uv.y);
    }

    private Vector2 Nearest2DPositionUV(Vector3 v)
    {
        Vector2 position = Nearest2DPosition(v);
        float ratio_x = (position.x / (_drawing.GetComponent<RectTransform>().sizeDelta.x * this.transform.localScale.x)) + 0.5f;
        float ratio_y = (position.y / (_drawing.GetComponent<RectTransform>().sizeDelta.y * this.transform.localScale.y)) + 0.5f; 
        return new Vector2(ratio_x, ratio_y);
    }

    private void Drag_Movement(MLInputController controller, bool onDragged, Vector3 dragPosition)
    {
        if (triggerDown)
        {
            if (_inEditMode && _editable)
            {

                Vector2 uv = Nearest2DPositionUV(dragPosition);
                //Debug.Log("uv coordinates: " + uv);
                //if (InBoundsUV(uv))
                //    _textureManager.ContinuePaint(uv);

                if (_outOfBounds)
                {
                    if (InBoundsUV(uv))
                    {
                        if (ToolResumePoint != null) ToolResumePoint.Invoke(uv);
                        _outOfBounds = false;
                    }
                }
                else
                {
                    if (InBoundsUV(uv))
                        if (ToolContinuePoint != null) ToolContinuePoint.Invoke(uv); // continue painting
                    else
                    {
                        if (ToolPausePoint != null) ToolPausePoint.Invoke(); // pause painting
                        _outOfBounds = true;
                    }
                }

            }
            else
            {           
                transform.position = controller.Position;
                transform.rotation = controller.Orientation;

                transform.position += transform.forward * _offsetDistance;

                transform.position -= transform.right * _offsetPosition.x;
                transform.position -= transform.up * _offsetPosition.y;
                transform.position -= transform.forward * _offsetPosition.z;


                if (_otherCollider != null && _model == null)
                {
                    //if the collision still happens after the movement
                    bool collideWithLayers = _otherCollider.bounds.Intersects(_bounds.GetComponent<Collider>().bounds);
                    bool collideWithButtons = _otherCollider.bounds.Intersects(_closeButton.GetComponent<Collider>().bounds)
                        || _otherCollider.bounds.Intersects(_editButton.GetComponent<Collider>().bounds);

                    if (collideWithLayers || collideWithButtons)
                    {
                        //controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.High);
                        //controller.StopFeedbackPatternVibe();

                        GameObject otherObject = _otherCollider.gameObject;
                        Transform planeTransform = otherObject.transform;

                        transform.rotation = Quaternion.FromToRotation(transform.forward, planeTransform.forward) * transform.rotation;

                        //find the distance between the window and the pysical plane
                        Plane extendedPlane = new Plane(planeTransform.forward, planeTransform.position);
                        float distance = extendedPlane.GetDistanceToPoint(transform.position);
                        distance += _placementMargin;
                        if (!extendedPlane.GetSide(transform.position))
                        {
                            distance *= -1;
                        }

                        print(distance);

                        transform.position += transform.forward * distance;

                        print(transform.position);
                    }
                }
            }
        }
    }

    private void Drag_Stop(MLInputController controller, float triggerValue, Vector3 dragPosition)
    {
        triggerDown = false;
        if (_inEditMode && _editable)
        {
            if (ToolEndPoint != null) ToolEndPoint.Invoke();
            _outOfBounds = true;
        }
            
    }

    private void Collision_Enter(Collider other)
    {
        if (_otherCollider == null)
        {
            _otherCollider = other;
        }
    }

    private void Collision_Exit(Collider other)
    {
        // print("Collision Exit");
        if (_otherCollider == other)
        {
            _otherCollider = null;
        }
    }

    private void Edit_Triggered(MLInputController controller, float triggerValue, Vector3 dragPosition)
    {
        _inEditMode = !_inEditMode;
    }

    private void Close_Triggered(MLInputController controller, float triggerValue, Vector3 dragPosition)
    {
        Destroy(this.gameObject, 0.0f);
    }
    #endregion // Event Handlers
}
