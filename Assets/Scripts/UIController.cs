using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement root;
    private Label coordinates_label;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        coordinates_label = root.Q<Label>("current_coords");
        Button btnCDA = root.Q<Button>("CDA_mode");
        Button btnBresenham = root.Q<Button>("Bresenham_mode");
        Button btnWu = root.Q<Button>("Wu_mode");
        Button btnCircle = root.Q<Button>("circle_mode");
        Button btnEllipse = root.Q<Button>("ellipse_mode");
        Button btnParabola = root.Q<Button>("parabola_mode");
        Button btnHyperbola = root.Q<Button>("hyperbola_mode");
        Button btnHermite = root.Q<Button>("hermite_mode");
        Button btnBezier = root.Q<Button>("bezier_mode");
        Button btnBSpline = root.Q<Button>("bspline_mode");

        Button btnClearScreen = root.Q<Button>("clear_screen");
        Button btnReset = root.Q<Button>("reset_mode");

        btnCDA.clicked += SelectCDA;
        btnBresenham.clicked += SelectBresenham;
        btnWu.clicked += SelectWu;
        btnCircle.clicked += SelectCircle;
        btnEllipse.clicked += SelectEllipse;
        btnParabola.clicked += SelectParabola;
        btnHyperbola.clicked += SelectHyperbola;
        btnHermite.clicked += SelectHermite;
        btnBezier.clicked += SelectBezier;
        btnBSpline.clicked += SelectBSpline;

        btnReset.clicked += Reset;
        btnClearScreen.clicked += ClearScreen;
    }

    void Update()
    {
        if (RaycastController.coords != null)
        {
            coordinates_label.text = "(" + RaycastController.coords[0].ToString() + "; " + RaycastController.coords[1].ToString() + ")";    
        }
        else
        {
            coordinates_label.text = "Nothing on board selected";
        }
    }

    private void SelectCDA()
    {
        Reset();
        GameController.mode = GameController.Mode.LineCDA;
    }

    private void SelectBresenham()
    {
        Reset();
        GameController.mode = GameController.Mode.LineBresenham;
    }

    private void SelectWu()
    {
        Reset();
        GameController.mode = GameController.Mode.LineWu;
    }

    private void SelectCircle()
    {
        Reset();
        GameController.mode = GameController.Mode.Circle;
    }

    private void SelectEllipse()
    {
        Reset();
        GameController.mode = GameController.Mode.Ellipse;
    }

    private void SelectParabola()
    {
        Reset();
        GameController.mode = GameController.Mode.Parabola;
    }

    private void SelectHyperbola()
    {
        Reset();
        GameController.mode = GameController.Mode.Hyperbola;
    }

    private void SelectHermite()
    {
        Reset();
        GameController.mode = GameController.Mode.Hermite;
    }

    private void SelectBezier()
    {
        Reset();
        GameController.mode = GameController.Mode.Bezier;
    }

    private void SelectBSpline()
    {
        Reset();
        GameController.mode = GameController.Mode.Bspline;
    }

    private void Reset()
    {
        GameController.mode = GameController.Mode.None;
        GameController.ClearSelectedPixels();
    }

    private void ClearScreen()
    {
        GameController.ClearScreen();
    }
}
