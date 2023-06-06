using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    [RequireComponent(typeof(ISerializable))]
    public class Observer : MonoBehaviour, IObservable
    {
        [SerializeField] private PhysicsRaycaster _raycaster;
        [SerializeField] private float _delayBetweenRepeat;
        [SerializeField] private string _fileName;
        [SerializeField] private bool _needSerialize;
        [SerializeField] private bool _needDeserilize;

        private ISerializable _handler;
        private List<string> _output;

        public event Action<Coordinate> NextStepReady;

        private void Awake()
        {
            _handler = GetComponent<ISerializable>();
        }

        private void OnEnable()
        {
            _handler.StepOver += OnStepOver;
        }

        private void OnDisable()
        {
            _handler.StepOver -= OnStepOver;
        }

        private void Start()
        {
            if (_needSerialize && !_needDeserilize)
            {
                File.Delete(_fileName + ".txt");
            }

            if (_needDeserilize && !_needSerialize)
            {
                _raycaster.enabled = false;
                var output = Deserialize();
                _output = output.Split(Environment.NewLine).ToList();
                OnStepOver();
            }
        }

        public async Task Serialize(string input)
        {
            if (!_needSerialize && _needDeserilize)
            {
                return;
            }

            await using var fileStream = new FileStream(_fileName + ".txt", FileMode.Append);
            await using var streamWriter = new StreamWriter(fileStream);

            await streamWriter.WriteLineAsync(input);
        }

        private string Deserialize()
        {
            if (!File.Exists(_fileName + ".txt"))
            {
                return null;
            }

            using var fileStream = new FileStream(_fileName + ".txt", FileMode.Open);
            using var streamReader = new StreamReader(fileStream);

            var builder = new StringBuilder();

            while (!streamReader.EndOfStream)
            {
                builder.AppendLine(streamReader.ReadLine());
            }

            return builder.ToString();
        }

        private void OnStepOver()
        {
            if (!_needDeserilize && _needSerialize)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_output[0]))
            {
                _needDeserilize = false;
                _needSerialize = true;
                Debug.Log("Deserialize end");
                _raycaster.enabled = true;
                return;
            }
            
            StartCoroutine(RepeatGame(_output[0]));
            _output.RemoveAt(0);
        }

        private IEnumerator RepeatGame(string input)
        {
            const string playerCommandPattern = @"Player (\d+) (Move|Click|Remove)";
            const string pairOfDigitsPattern = @"(\d+),(\d+)";

            Coordinate destination = default;

            yield return new WaitForSeconds(_delayBetweenRepeat);
            
            var playerCommandMatch = Regex.Match(input, playerCommandPattern);
            var playerIndex = int.Parse(playerCommandMatch.Groups[1].Value);
            
            var command = playerCommandMatch.Groups[2].Value;

            var pairsOfDigits = Regex.Matches(input, pairOfDigitsPattern);

            var origin = (
                int.Parse(pairsOfDigits[0].Groups[1].Value),
                int.Parse(pairsOfDigits[0].Groups[2].Value)).ToCoordinate();

            if (command == "Move")
            {
                destination = (
                    int.Parse(pairsOfDigits[1].Groups[1].Value),
                    int.Parse(pairsOfDigits[1].Groups[2].Value)).ToCoordinate();
            }

            switch (command)
            {
                case "Click":
                    Debug.Log($"Player {playerIndex} {command} to {origin}");
                    NextStepReady?.Invoke(origin);
                    break;

                case "Remove":
                    Debug.Log($"Player {playerIndex} {command} chip at {origin}");
                    NextStepReady?.Invoke(new Coordinate(-1, -1));
                    break;
                
                case "Move":
                    Debug.Log($"Player {playerIndex} {command} from {origin} to {destination}");
                    NextStepReady?.Invoke(destination);
                    break;
                
                default:
                    throw new NullReferenceException("Action is null");
            }
        }
    }
}