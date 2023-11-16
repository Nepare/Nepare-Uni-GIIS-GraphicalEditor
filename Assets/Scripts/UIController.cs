using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement root;
    private Label coordinates_label;
    private bool is3DActive = false;
    private int x = 0, y = 0, z = 0;

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
        Button btnSpawnCube = root.Q<Button>("spawn_cube");
        
        Button btnRotate = root.Q<Button>("Rotate");
        Button btnMove = root.Q<Button>("Move");
        Button btnScale = root.Q<Button>("Scale");
        Button btnPerspect = root.Q<Button>("Perspect");
        Button btnReflect = root.Q<Button>("Reflect");


        Button btnClearScreen = root.Q<Button>("clear_screen");
        Button btnReset = root.Q<Button>("reset_mode");

        TextField txtBoxBezierParameter = root.Q<TextField>("txtBoxBezierParameter");

        TextField txtBoxX = root.Q<TextField>("txtboxX");
        TextField txtBoxY = root.Q<TextField>("txtboxY");
        TextField txtBoxZ = root.Q<TextField>("txtboxZ");

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
        
        btnSpawnCube.clicked += SpawnCube;
        btnMove.clicked += MoveCube;
        btnRotate.clicked += RotateCube;
        btnScale.clicked += ScaleCube;
        btnPerspect.clicked += PerspectCube;
        btnReflect.clicked += ReflectCube;
        
        btnReset.clicked += Reset;
        btnClearScreen.clicked += ClearScreen;

        txtBoxBezierParameter.RegisterValueChangedCallback(evt => 
        {
            try { Convert.ToInt32(txtBoxBezierParameter.value); }
            catch { return; }
            if (Convert.ToInt32(txtBoxBezierParameter.value) >= 4)
                EventManager.SendBSplineParameterChanged(Convert.ToInt32(txtBoxBezierParameter.value));
        });

        txtBoxX.RegisterValueChangedCallback(evt => 
        {   try { Convert.ToInt32(txtBoxX.value); } catch { x = 0; return; }
            x = Convert.ToInt32(txtBoxX.value); });
        txtBoxY.RegisterValueChangedCallback(evt => 
        {   try { Convert.ToInt32(txtBoxY.value); } catch { y = 0; return; }
            y = Convert.ToInt32(txtBoxY.value); });
        txtBoxZ.RegisterValueChangedCallback(evt => 
        {   try { Convert.ToInt32(txtBoxZ.value); } catch { z = 0; return; }
            z = Convert.ToInt32(txtBoxZ.value); });
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

    private void SpawnCube()
    {
        Reset3D();
        ClearScreen();
        is3DActive = true;
        GameController.SpawnCube();
    }

    private void MoveCube()
    {
        ClearScreen();
        GameController.SendValues(true, false, false, false, false, x, y, z);
        GameController.RenderCube();
    }

    private void RotateCube()
    {
        ClearScreen();
        GameController.SendValues(false, true, false, false, false, x, y, z);
        GameController.RenderCube();
    }

    private void ScaleCube()
    {
        ClearScreen();
        GameController.SendValues(false, false, true, false, false, x, y, z);
        GameController.RenderCube();
    }

    private void PerspectCube()
    {
        ClearScreen();
        GameController.SendValues(false, false, false, true, false, x, y, z);
        GameController.RenderCube();
    }

    private void ReflectCube()
    {
        ClearScreen();
        GameController.SendValues(false, false, false, false, true, x, y, z);
        GameController.RenderCube();
    }

    private void Reset()
    {
        GameController.mode = GameController.Mode.None;
        is3DActive = false;
        GameController.ClearSelectedPixels();
    }

    private void Reset3D()
    {
        GameController.mode = GameController.Mode.None;
        GameController.ClearSelectedPixels();
    }

    private void ClearScreen()
    {
        GameController.ClearScreen();
    }
}
