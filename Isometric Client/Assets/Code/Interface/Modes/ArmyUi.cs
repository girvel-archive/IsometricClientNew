using System;
using System.Linq;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;

namespace Assets.Code.Interface.Modes
{
    public class ArmyUi : IModeUi
    {
        private Vector _lastPosition;

        private ArmyDto[] _lastArmiesDto;

        private int _currentArmyIndex;



        public void Refresh()
        {
            GameUi.Current.Clear();

            if (BuildingsManager.Current.SelectedBuilding != null)
            {
                SelectCell(BuildingsManager.Current.SelectedBuilding.Position);
            }
        }

        public void SelectCell(Vector position)
        {
            if (position != _lastPosition)
            {
                _currentArmyIndex = 0;
                _lastPosition = position;
            }

            _lastArmiesDto = NetManager.Current.GetArmiesInfo(position);

            ShowPreviousData();
        }

        public void HighlightCell(Vector position)
        {
            InformationPanel.Current.ShowInformation(
                NetManager.Current.GetArmiesInfo(position));
        }

        public void Update(TimeSpan deltaTime)
        {
            
        }

        public void ShowPreviousData()
        {
            GameUi.Current.Clear();

            if (_lastArmiesDto == null)
            {
                return;
            }

            if (BuildingsManager.Current[_lastPosition].Army == null) return;

            if (!_lastArmiesDto.Any())
            {
                return;
            }

            if (_lastArmiesDto.Length - 1 < _currentArmyIndex)
            {
                _currentArmyIndex = 0;
            }

            if (_lastArmiesDto.Length > 1)
            {
                TableManager.Current.SetButton(
                    0, 0,
                    Sprites.Current.Left,
                    b =>
                    {
                        _currentArmyIndex = (_currentArmyIndex + _lastArmiesDto.Length - 1) % _lastArmiesDto.Length;
                    },
                    "Выбрать предыдущую армию");

                TableManager.Current.SetButton(
                    1, 0,
                    Sprites.Current.Right,
                    b =>
                    {
                        _currentArmyIndex = (_currentArmyIndex + 1) % _lastArmiesDto.Length;
                    },
                    "Выбрать следующую армию");
            }

            if (_lastArmiesDto[_currentArmyIndex].IsControllable)
            {
                TableManager.Current.SetButton(
                    4, 0,
                    Sprites.Current.Move,
                    b =>
                    {
                        GameUi.Current.SetSelectingTargetAction(to =>
                        {
                            NetManager.Current.MoveArmy(_lastPosition, to, _currentArmyIndex);
                        });
                    },
                    "Переместить армию");

                TableManager.Current.SetButton(
                    3, 0,
                    Sprites.Current.DestroyBuilding,
                    b =>
                    {
                        BuildingsManager.Current.SetUpgrade(
                            _lastPosition, 
                            "Plain", 
                            NetManager.Current.LootBuilding(_lastPosition, _currentArmyIndex));
                    },
                    "Уничтожить здание");
            }

            InformationPanel.Current.ShowInformation(_lastArmiesDto[_currentArmyIndex]);
        }

        public void Clear()
        {
            
        }
    }
}